using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flxSharp.flxSharp;

namespace fliXNA_xbox
{
    public class FlxList
	{
		/**
		 * Stores a reference to a <code>FlxObject</code>.
		 */
		public FlxObject Object;
		/**
		 * Stores a reference to the next link in the list.
		 */
		public FlxList next;

		/**
		 * Creates a new link, and sets <code>object</code> and <code>next</code> to <code>null</code>.
		 */
		public FlxList()
		{
			Object = null;
			next = null;
		}

		/**
		 * Clean up memory.
		 */
		public void destroy()
		{
		    Object = null;
			if(next != null)
				next.destroy();
			next = null;
		}
	}
}
