using System;
using UnityEngine;

namespace Soomla.Store
{
	public static class IOS_ErrorCodes
	{
		public static void CheckAndThrowException(int error)
		{
			if (error == IOS_ErrorCodes.EXCEPTION_ITEM_NOT_FOUND)
			{
				UnityEngine.Debug.Log("SOOMLA/UNITY Got VirtualItemNotFoundException exception from 'extern C'");
				throw new VirtualItemNotFoundException();
			}
			if (error == IOS_ErrorCodes.EXCEPTION_INSUFFICIENT_FUNDS)
			{
				UnityEngine.Debug.Log("SOOMLA/UNITY Got InsufficientFundsException exception from 'extern C'");
				throw new InsufficientFundsException();
			}
			if (error == IOS_ErrorCodes.EXCEPTION_NOT_ENOUGH_GOODS)
			{
				UnityEngine.Debug.Log("SOOMLA/UNITY Got NotEnoughGoodsException exception from 'extern C'");
				throw new NotEnoughGoodsException();
			}
		}

		public static int NO_ERROR;

		public static int EXCEPTION_ITEM_NOT_FOUND = -101;

		public static int EXCEPTION_INSUFFICIENT_FUNDS = -102;

		public static int EXCEPTION_NOT_ENOUGH_GOODS = -103;
	}
}
