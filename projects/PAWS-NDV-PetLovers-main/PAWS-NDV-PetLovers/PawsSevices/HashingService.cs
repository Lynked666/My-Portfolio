using System;
using System.Security.Cryptography;
using System.Text;

namespace PAWS_NDV_PetLovers.PawsSevices
{
    public class HashingService
    {
        public static string HashData(string userData)
        {
            // Create instance Algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                //copnvert to byte array
                byte[] inputBytes = Encoding.UTF8.GetBytes(userData);

                //compute the array using sha256
                byte[] hashBytes = sha256.ComputeHash(inputBytes);


                StringBuilder builder = new StringBuilder(); 

                for(int i =0; i < 10; i++)
                {
                    //convert each bytes to its hexadecimal representation
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
