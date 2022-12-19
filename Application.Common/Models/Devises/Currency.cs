using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Application.Common.Models.Devises
{
    public class Currency : IDatabaseModel
    {
        private string m_Name;

        public Currency()
        {
            ExchangeRates = new ObservableCollection<ExchangeRate>();
            RateID = Guid.NewGuid();
            Name = "";
        }

        public int ID { get; set; }

        public string Name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                OnPropertyChanged();
            }
        }

        public Guid RateID { get; set; }

        public ObservableCollection<ExchangeRate> ExchangeRates { get; set; }

        public int ZIndex => 20;

        public double Convert(string _DeviseName, double _Value)
        {
            var exchangeRate = ExchangeRates.FirstOrDefault(_ => _.Currency.Name == _DeviseName);

            if (exchangeRate == null)
                return _Value;

            return Math.Round(exchangeRate.Rate * _Value, 2);
        }
        public double Convert(Currency _Currency, double _Value)
        {
            var exchangeRate = ExchangeRates.FirstOrDefault(_ => _.Currency == _Currency);

            if (exchangeRate == null)
                return _Value;

            return Math.Round(exchangeRate.Rate * _Value, 2);
        }

        public object GetID()
        {
            return ID;
        }

        public object Clone()
        {
            return (Currency) MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
