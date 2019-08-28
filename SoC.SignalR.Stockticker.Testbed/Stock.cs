using System;

namespace SoC.SignalR.Stockticker.Testbed
{
    public class Stock
    {
        private decimal _price;

        public string Symbol { get; set; }

        public decimal Price
        {
            get
            {
                return this._price;
            }
            set
            {
                if (this._price == value)
                {
                    return;
                }

                this._price = value;

                if (this.DayOpen == 0)
                {
                    this.DayOpen = this._price;
                }
            }
        }

        public decimal DayOpen { get; private set; }

        public decimal Change
        {
            get
            {
                return this.Price - this.DayOpen;
            }
        }

        public double PercentChange
        {
            get
            {
                return (double)Math.Round(this.Change / this.Price, 4);
            }
        }
    }
}
