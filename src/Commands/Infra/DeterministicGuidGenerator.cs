using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MilkmenUnion.Commands.Infra
{
    /// <summary>
    /// Credits to http://github.com/damianh
    /// Not my code but hey it works
    /// </summary>
    public class DeterministicGuidGenerator
    {
        private readonly byte[] _namespaceBytes;

        /// <summary>
        ///     Initializes a new instance of <see cref="T:SqlStreamStore.Infrastructure.DeterministicGuidGenerator" />
        /// </summary>
        /// <param name="guidNameSpace">
        ///     A namespace that ensures that the GUID generated with this instance
        ///     do not collided with other generators. Your application should define
        ///     it's namespace as a constant.
        /// </param>
        public DeterministicGuidGenerator(Guid guidNameSpace)
        {
            this._namespaceBytes = guidNameSpace.ToByteArray();
            DeterministicGuidGenerator.SwapByteOrder(this._namespaceBytes);
        }

        /// <summary>Creates a deterministic GUID.</summary>
        /// <param name="source">A source to generate the GUID from.</param>
        /// <returns>A deterministically generated GUID.</returns>
        public Guid Create(byte[] source)
        {
            byte[] hash;
            using (SHA1 shA1 = SHA1.Create())
            {
                shA1.TransformBlock(this._namespaceBytes, 0, this._namespaceBytes.Length, (byte[])null, 0);
                shA1.TransformFinalBlock(source, 0, source.Length);
                hash = shA1.Hash;
            }
            byte[] numArray = new byte[16];
            Array.Copy((Array)hash, 0, (Array)numArray, 0, 16);
            numArray[6] = (byte)((int)numArray[6] & 15 | 80);
            numArray[8] = (byte)((int)numArray[8] & 63 | 128);
            DeterministicGuidGenerator.SwapByteOrder(numArray);
            return new Guid(numArray);
        }

        private static void SwapByteOrder(byte[] guid)
        {
            DeterministicGuidGenerator.SwapBytes(guid, 0, 3);
            DeterministicGuidGenerator.SwapBytes(guid, 1, 2);
            DeterministicGuidGenerator.SwapBytes(guid, 4, 5);
            DeterministicGuidGenerator.SwapBytes(guid, 6, 7);
        }

        private static void SwapBytes(byte[] guid, int left, int right)
        {
            byte num = guid[left];
            guid[left] = guid[right];
            guid[right] = num;
        }
    }
}
