using Application.Common.Models.Items;
using System;

namespace Application.Common.Models.Providers
{
    public sealed class Provider : ItemBase
    {
        private string m_Commentary;
        private double m_PriceTransport;
        private string m_Manager;
        private string m_ClientNumber;
        private string m_SiteWeb;
        private string m_Mail;
        private string m_PrivatePhone;
        private string m_Phone;
        private string m_Number;
        private string m_Name;
        private string m_Address;
        private string m_Locality;
        private string m_Npa;
        private string m_Country;
        private string m_Fax;

        public Provider()
        {
            Default();
        }

        public Guid GroupProviderID { get; set; }

        public string Number
        {
            get => m_Number;
            set
            {
                var oldValue = m_Number;
                m_Number = value;
                OnPropertyChanged(nameof(Number), oldValue, m_Number);
            }
        }

        public string Name
        {
            get => m_Name;
            set
            {
                var oldValue = m_Name;
                m_Name = value;
                OnPropertyChanged(nameof(Name), oldValue, m_Name);
            }
        }

        public string Address
        {
            get => m_Address;
            set
            {
                var oldValue = m_Address;
                m_Address = value;
                OnPropertyChanged(nameof(Address), oldValue, m_Address);
            }
        }

        public string Locality
        {
            get => m_Locality;
            set
            {
                var oldValue = m_Locality;
                m_Locality = value;
                OnPropertyChanged(nameof(Locality), oldValue, m_Locality);
            }
        }

        public string NPA
        {
            get => m_Npa;
            set
            {
                var oldValue = m_Npa;
                m_Npa = value;
                OnPropertyChanged(nameof(NPA), oldValue, m_Npa);
            }
        }

        public string Country
        {
            get => m_Country;
            set
            {
                var oldValue = m_Country;
                m_Country = value;
                OnPropertyChanged(nameof(Country), oldValue, m_Country);
            }
        }

        public string Fax
        {
            get => m_Fax;
            set
            {
                var oldValue = m_Fax;
                m_Fax = value;
                OnPropertyChanged(nameof(Fax), oldValue, m_Fax);
            }
        }

        public string Phone
        {
            get => m_Phone;
            set
            {
                var oldValue = m_Fax;
                m_Fax = value;
                OnPropertyChanged(nameof(Fax), oldValue, m_Fax);
            }
        }

        public string PrivatePhone
        {
            get => m_PrivatePhone;
            set
            {
                var oldValue = m_PrivatePhone;
                m_PrivatePhone = value;
                OnPropertyChanged(nameof(PrivatePhone), oldValue, m_PrivatePhone);
            }
        }

        public string Mail
        {
            get => m_Mail;
            set
            {
                var oldValue = m_Mail;
                m_Mail = value;
                OnPropertyChanged(nameof(Mail), oldValue, m_Mail);
            }
        }

        public string SiteWeb
        {
            get => m_SiteWeb;
            set
            {
                var oldValue = m_SiteWeb;
                m_SiteWeb = value;
                OnPropertyChanged(nameof(SiteWeb), oldValue, m_SiteWeb);
            }
        }

        public string ClientNumber
        {
            get => m_ClientNumber;
            set
            {
                var oldValue = m_ClientNumber;
                m_ClientNumber = value;
                OnPropertyChanged(nameof(ClientNumber), oldValue, m_ClientNumber);
            }
        }

        public string Manager
        {
            get => m_Manager;
            set
            {
                var oldValue = m_Manager;
                m_Manager = value;
                OnPropertyChanged(nameof(Manager), oldValue, m_Manager);
            }
        }

        public double PriceTransport
        {
            get => m_PriceTransport;
            set
            {
                var oldValue = m_PriceTransport;
                m_PriceTransport = value;
                OnPropertyChanged(nameof(PriceTransport), oldValue, m_PriceTransport);
            }
        }

        public string Commentary
        {
            get => m_Commentary;
            set
            {
                var oldValue = m_Commentary;
                m_Commentary = value;
                OnPropertyChanged(nameof(Commentary), oldValue, m_Commentary);
            }
        }
        public override int ZIndex => 5;

        public void CopyTo(Provider _Provider)
        {
            Address = _Provider.Address;
            Name = _Provider.Name;
            Country = _Provider.Country;
            Fax = _Provider.Fax;
            Locality = _Provider.Locality;
            Number = _Provider.Number;
            ClientNumber = _Provider.ClientNumber;
            Commentary = _Provider.Commentary;
            Mail = _Provider.Mail;
            Manager = _Provider.Manager;
            NPA = _Provider.NPA;
            Phone = _Provider.Phone;
            PrivatePhone = _Provider.PrivatePhone;
            SiteWeb = _Provider.SiteWeb;
            GroupProviderID = _Provider.GroupProviderID;
            ID = _Provider.ID;
            PriceTransport = _Provider.PriceTransport;
        }

        public override void Default()
        {
            m_Commentary = "";
            m_PriceTransport = 0d;
            m_Manager = "";
            m_ClientNumber = "";
            m_SiteWeb = "";
            m_Mail = "";
            m_PrivatePhone = "";
            m_Phone = "";
            m_Number = "";
            m_Name = "";
            m_Address = "";
            m_Locality = "";
            m_Npa = "";
            m_Country = "";
            m_Fax = "";
            GroupProviderID = Guid.NewGuid();
        }

        public override string ToString()
        {
            return Name;
        }

        public override object Clone()
        {
            return (Provider)MemberwiseClone();
        }
    }
}
