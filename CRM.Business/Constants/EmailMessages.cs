namespace CRM.Business.Constants
{
    public static class EmailMessages
    {
        public const string RegistrationSubject = "Registration account on CRM";
        public const string RegistrationBody = "Congratulations on your registration. Please download google authenticator and scan QR";
        public const string QRCode = "<html><body><p><img src=\"{0}\"/></p></body></html>";
        public const string DeleteLeadSubject = "Delete account on CRM";
        public const string DeleteLeadBody = "you deleted your account, write to us for recovery.";
        public const string AccountAddedSubject = "New account added";
        public const string AccountAddedBody = "you have opened a new account in currency";
        public const string AccountDeleteSubject = "Account deleted";
        public const string AccountDeleteBody = "you have deleted your account in currency";
        public const string AccountRestoreSubject = "Account restored";
        public const string AccountRestoreBody = "you have restored your account in currency";
        public const string DepositSubject = "Deposit";
        public const string DepositBody = "you put on your account {0}";
        public const string WithdrawSubject = "Deposit";
        public const string WithdrawBody = "you have withdrawn from the account {0}";
        public const string TransferSubject = "Deposit";
        public const string TransferBody = "you have transferred {0} {1} from your account to your {2} account";
        public const string TwoFactorAuthSubject = "Confirm the operation by entering your pin";
        public const string TwoFactorAuthBody = "transaction/two-factor-authentication/{pinCode}";
    }
}