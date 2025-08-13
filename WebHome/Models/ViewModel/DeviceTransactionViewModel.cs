namespace WebHome.Models.ViewModel
{
    public class DeviceTransactionViewModel
    {
        public DefenceStatus? Defence { get; set; }
        public DeviceTransactionViewModel.UrgentEventDefinition? EventCode { get; set; }

        public DoorStatus? MainDoor { get; set; }

        public enum DefenceStatus
        {
            OffGuard = 0,
            OnGuard = 1,
            CallElevator = 2,
        }

        public enum UrgentEventDefinition
        {
            Clear = -1,
            火災 = 5,
            地震 = 14,
        }

        public enum DoorStatus
        {
            Opened = 0,
            Closed = 1,
        }
    }
}