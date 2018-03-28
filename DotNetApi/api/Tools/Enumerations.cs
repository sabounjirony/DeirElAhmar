namespace Tools
{
    public static class Enumerations
    {
        public enum ErrorType
        {
            Error = 1,
            Alert = 2,
            Information = 3
        }

        public enum ErrorSeverity
        {
            High = 1,
            Medium = 2,
            Low = 3
        }

        public enum DescriptionResources
        {
            Language = 1,
            System = 2,
            Exceptions = 3,
            Entity = 4,
            Stock = 5,
            ListOfValues = 6
        }

        public enum DateLimit
        {
            Today = 1,
            Yesterday = 2,
            ThisWeek = 3,
            PreviousWeek = 4,
            ThisMonth = 5,
            PreviousMonth = 6,
            ThisQuarter = 7,
            PreviousQuarter = 11,
            ThisHalfYear = 8,
            PreviousHalfYear = 13,
            ThisYear = 9,
            PreviousYear = 12,
            Open = 10
        }

        public enum DateInterval
        {
            Year = 1,
            Month = 2,
            Day = 3,
            Hour = 4,
            Minutes = 5,
            Seconds = 6
        }

        public enum Languages
        {
            English = 1,
            Arabic = 2,
            Frensh = 3
        }

        public enum ActionMode
        {
            Add = 1,
            Edit = 2,
            Delete = 3
        }

        public enum OrderStatus
        {
            Draft,
            Completed,
            Rejected,
            Canceled,
            Posted,
            Received,
            Validated,
            Authorized
        }

        public enum UnitsType
        {
            Stock = 1,
            Order = 2
        }

        public enum OrderDashboardView
        {
            WaitingAuthorization = 1,
            WaitingSecondAuthorization = 2,
            WaitingLateAuthorization = 3,
            ReadyForPosting = 4,
            WaitingReception = 5,
            WaitingValidation = 6,
            WaitingStorage = 7,
            MissingReceipt = 8,
            MissingBill = 9
        }

        public enum DistributionDashboardView
        {
            WaitingAuthorization = 1,
            WaitingLateAuthorization = 2,
            WaitingExecution = 3,
            MissingReceipt = 4
        }

        public enum OperationDashboardView
        {
            WaitingExecution = 1
        }

        public enum InventoryDashboardView
        {
            Active = 1,
            History = 2
        }

        public enum StockDashboardView
        {
            BelowTreshold = 1,
            AboveTreshold = 2,
            OnOrder = 3
        }

        public enum TokenType
        {
            String = 1,
            Integer = 2,
            Long = 3,
            Double = 4,
            Date = 5
        }

        public enum ProductDisplayType
        {
            FullCode = 1,
            OwnCode = 2,
            FullName = 3
        }

        public enum OperationType
        {
            Order = 1,
            Distribution = 2,
            Operation = 3,
            Partition = 4
        }

        public static string[] ArActiveOrderStatus = { "DR", "AUTH", "SAUTH", "PO", "RC", "PRC", "VAL", "PVAL" };
        public static string[] ArInActiveOrderStatus = { "CM", "CN", "RJ", "DEL" };
        public static string[] ArNoNotificationsOrderStatus = { "CN", "RJ", "DEL" };

        public static string[] ArActiveDistributionStatus = { "DR", "AUTH" };
        public static string[] ArInActiveDistributionStatus = { "CM", "RJ", "DEL" };
        public static string[] ArNoNotificationsDistributionStatus = { "RJ", "DEL" };

        public static string[] ArActiveOperationStatus = { "DR" };
        public static string[] ArInActiveOperationStatus = { "CM", "DEL" };

        public static string[] ArActivePartitionStatus = { "DR" };
        public static string[] ArInActivePartitionStatus = { "CM", "DEL" };

        public static string[] ArAllStockStatusButHistory = { "A", "D", "G", "I", "L", "M", "N", "R" };
        public static string[] ArActiveLocationStockStatus = { "A", "I", "D", "G", "R" };
        public static string[] ArAvailableStockStatus = { "A", "N", "R" };
        public static string[] ArActiveStockStatus = { "I" };
        public static string[] ArAllowMaintenanceStockStatus = { "A", "I", "N" };
        public static string[] ArAllowLostStockStatus = { "A", "I", "N" };
        public static string[] ArAllowInventoryStockStatus = { "A", "I", "N" };

        public static string[] ArCountedInventoryStatus = { "C", "S" };
        public static string[] ArUnCountedInventoryStatus = { "U" };
    }
}