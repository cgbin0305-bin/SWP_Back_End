
using System.Collections.Generic;
namespace API.Helper
{
    public class SendAndRetrieveRandomOtpCode
    {
        public Dictionary<string, string> otpCodeDictionary = new Dictionary<string, string>();

        public string GenerateRandomOtpCode()
        {
            // Generate a random 6-digit OTP code
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        public void StoreOtpCodeForUser(string userEmail, string otpCode)
        {
            // Store the OTP code in a dictionary with the user's ID as the key
            otpCodeDictionary[userEmail] = otpCode;
        }

        public string GetStoredOtpCodeForUser(string userEmail)
        {
            // Retrieve the OTP code from the dictionary based on the user's ID
            if (otpCodeDictionary.ContainsKey(userEmail))
            {
                return otpCodeDictionary[userEmail];
            }

            // Return null or an empty string if the user's OTP code is not found
            return null;
        }
    }
}