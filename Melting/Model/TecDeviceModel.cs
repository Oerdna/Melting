using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Melting.ServiceSender.Data;
using ServiceSender.Data;
using ServiceSender.ThreadSender;
using System;

namespace Melting.Model
{
    public partial class TecDeviceModel: ObservableObject
    {

        private readonly ThreadSender? sender;

        public TecDeviceModel(ThreadSender sender)
        {
            this.sender = sender;
            this.sender.AchivedResult += this.HandlerTec;
        }

        [ObservableProperty]
        private float sensorTemp= 1.0f;

        [ObservableProperty]
        private float devTarget = 1.0f;

        [ObservableProperty]
        private float targetTemp = 1.0f;

        [ObservableProperty]
        private bool isEnablePeltie = false;

        [ObservableProperty]
        private bool isEnableCooler = false;

        [ObservableProperty]
        private float coefKp = 1.0f;

        [ObservableProperty]
        private float coefKi = 1.0f;

        [ObservableProperty]
        private float coefKd = 1.0f;

        [ObservableProperty]
        private float coefKa = 1.0f;

        [ObservableProperty]
        private float devKp = 1.0f;

        [ObservableProperty]
        private float devKi = 1.0f;

        [ObservableProperty]
        private float devKd = 1.0f;

        [ObservableProperty]
        private float devKa = 1.0f;

        public static (float kp, float ki, float kd, float ka) UnpuckPidCoef(byte[] array)
        {
            if(array.Length != 16)
            {
                return (0, 0, 0, 0);
            }
            else
            {
                byte[] b_kp = new byte[4];
                Array.Copy(array, b_kp, 4);
                float kp = BitConverter.ToSingle(b_kp);
                byte[] b_ki = new byte[4];
                Array.Copy(array, 4, b_ki, 0, 4);
                float ki = BitConverter.ToSingle(b_ki);
                byte[] b_kd = new byte[4];
                Array.Copy(array, 8, b_kd, 0, 4);
                float kd = BitConverter.ToSingle(b_kd);
                byte[] b_ka = new byte[4];
                Array.Copy(array, 12, b_ka, 0, 4);
                float ka = BitConverter.ToSingle(b_ka);
                return (kp, ki, kd, ka);
            } 
        }

        public static byte[] PuckPidToBytes(float kp, float ki, float kd, float ka)
        {
            byte[] payload = new byte[16];
            byte[] b_kp = BitConverter.GetBytes(kp);
            Array.Copy(b_kp, payload, b_kp.Length);
            byte[] b_ki = BitConverter.GetBytes(ki);
            Array.Copy(b_ki, 0, payload, 4, b_ki.Length);
            byte[] b_kd = BitConverter.GetBytes(kd);
            Array.Copy(b_kd, 0, payload, 8, b_kd.Length);
            byte[] b_ka = BitConverter.GetBytes(ka);
            Array.Copy(b_ka, 0, payload, 12, b_ka.Length);
            return payload;
        }

        [RelayCommand]
        private void SetPIDCoefcient()
        {
            CommandWord set_pid_word = new CommandWord(1, 0, 2, 16);
            byte[] payload = PuckPidToBytes(CoefKp, CoefKi, CoefKd, CoefKa);
            CommandData cmd_set_pid = new CommandData(set_pid_word, payload);
            sender?.PassCommand(cmd_set_pid);
        }

        [RelayCommand]
        private void GetPIDCoefcient()
        {
            CommandWord get_pid_word = new CommandWord(1, 1, 2, 16);
            CommandData cmd_get_pid = new CommandData(get_pid_word);
            sender?.PassCommand(cmd_get_pid);
        }

        [RelayCommand]
        private void SetTarget()
        {
            CommandWord word_set_target_temp = new CommandWord(1, 0, 1, 4);
            byte[] payload = BitConverter.GetBytes(TargetTemp);
            CommandData cmd_set_target_temp = new CommandData(word_set_target_temp, payload);
            sender?.PassCommand(cmd_set_target_temp);
        }

        [RelayCommand]
        private void GetTarget()
        {
            CommandWord word_get_target_temp = new CommandWord(1, 1, 1, 4);
            CommandData cmd_get_target_temp = new CommandData(word_get_target_temp);
            sender?.PassCommand(cmd_get_target_temp);
        }

        [RelayCommand]
        private void EnablePeltie(bool? isactive)
        {
            if (isactive is not null)
            {
                CommandWord word_peltie = new CommandWord(1, 0, 0, 2);
                byte[] payload = new byte[2];
                payload[0] = 0x1;
                payload[1] = (bool)isactive ? (byte)0x1 : (byte)0x0;
                CommandData cmd_peltie = new CommandData(word_peltie, payload);
                sender?.PassCommand(cmd_peltie);
            }
        }

        [RelayCommand]
        private void EnableCooler(bool? isactive)
        {
            if (isactive is not null)
            {
                CommandWord word_cooler = new CommandWord(1, 0, 0, 2);
                byte[] payload = new byte[2];
                payload[0] = 0x2;
                payload[1] = (bool)isactive ? (byte)0x1 : (byte)0x0;
                CommandData cmd_cooler = new CommandData(word_cooler, payload);
                sender?.PassCommand(cmd_cooler);
            }
        }

        [RelayCommand]
        private void GetSensTemp()
        {
            CommandWord word_get_temp = new CommandWord(1, 1, 5, 4);
            CommandData cmd_get_temp = new CommandData(word_get_temp);
            sender?.PassCommand(cmd_get_temp);
        }
        

        // нужна подписка на sender
        private void HandlerTec(object sender, ResponseData e)
        {
            if (e.IsSuccess)
            {
                switch (e.BoundCommand.Command.SubAddr)
                {
                    case 1:
                        {
                            if (e.BoundCommand.Command.Direction == 1)
                            {
                                if (e.BoundCommand.Command.NumOfWords == e.Data!.Length)
                                    DevTarget = BitConverter.ToSingle(e.Data);
                            }
                            else
                            {
                                GetTarget();
                            }
                            break;
                        }
                    case 2:
                        {
                            if (e.BoundCommand.Command.Direction == 1)
                            {
                                if (e.BoundCommand.Command.NumOfWords == e.Data!.Length)
                                    (DevKp, DevKi, DevKd, DevKa) = UnpuckPidCoef(e.Data!);
                            }
                            else
                            {
                                GetPIDCoefcient();
                            }
                            break;
                        }
                    case 5:
                        {
                            if(e.BoundCommand.Command.NumOfWords == e.Data!.Length)
                                SensorTemp = BitConverter.ToSingle(e.Data);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

    }
}
