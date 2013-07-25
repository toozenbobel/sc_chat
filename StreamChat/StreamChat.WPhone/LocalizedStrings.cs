using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreamChat.WPhone.Resources;

namespace StreamChat.WPhone
{
	public class LocalizedStrings
    {
        private static readonly AppResources _localizedResources = new AppResources();

		public AppResources Strings
        {
            get
            {
                return _localizedResources;
            }
        }
    }
}
