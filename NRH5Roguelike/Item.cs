using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRH5Roguelike.Object
{
	class Item
	{
        // Data fields
        private string name;
        private int weight;
        // -1 means "pricelss", 0 means "worthless", and all higher-than-zero
        // values represents the item's value in gold
        private int value;
	}
}
