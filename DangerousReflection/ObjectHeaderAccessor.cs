using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DangerousReflection {
	/// <summary>
	/// Object header accessor
	/// This accessor is so danger:
	/// - you can't lock the object used with it
	/// - it may cause program crash (not confirmed)
	/// </summary>
	internal static class ObjectHeaderAccessor {
		/// <summary>
		/// least 26 bit
		/// </summary>
		public const int MaxIndex = 0x3ffffff;

		/// <summary>
		/// Use to write readonly region
		/// </summary>
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress,
			 uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		/// <summary>
		/// Get index value of object header
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIndex(object obj) {
			unsafe
			{
				var objRef = __makeref(obj);
				var typePtr = **(IntPtr**)(&objRef); // address of obj;
				var syncBlock = *(int*)(typePtr - sizeof(int));
				var cacheIndex = syncBlock & MaxIndex;
				return cacheIndex;
			}
		}

		/// <summary>
		/// Set index value of object hedaer
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetIndex(object obj, int index) {
			unsafe
			{
				var objRef = __makeref(obj);
				var typePtr = **(IntPtr**)(&objRef); // address of obj;
				var syncBlockPtr = (int*)(typePtr - sizeof(int));
				var syncBlock = *syncBlockPtr;
				var newSyncBlock = (syncBlock & ~MaxIndex) | (index & MaxIndex);
				uint oldProtect;
				if (VirtualProtect((IntPtr)syncBlockPtr, sizeof(int), 0x40, out oldProtect)) {
					*syncBlockPtr = newSyncBlock;
					VirtualProtect((IntPtr)syncBlockPtr, sizeof(int), oldProtect, out oldProtect);
				}
			}
		}
	}
}
