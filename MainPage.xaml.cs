﻿using Microsoft.Maui.Handlers;
using System.Net;
using System.Text.Json;

namespace przelicznik
{
    public partial class MainPage : ContentPage
    {
        public class Currency
        {
            public string? code { get; set; }
            public IList<Rate> rates { get; set; }
        }

        public class Rate
        {
            public double? mid { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnConvertClicked(object sender, EventArgs e)
        {
            string baseCurrency = pickerSource.SelectedItem.ToString();
            string targetCurrency = pickerTarget.SelectedItem.ToString();
            double amount = double.Parse(amountEntry.Text);

            double exchangeRate = GetExchangeRate(baseCurrency, targetCurrency);
            double result = amount * exchangeRate;

            resultLabel.Text = $"Wynik: {result:F2} {targetCurrency}";
        }

        private double GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            if (baseCurrency == "PLN")
            {
                return GetExchangeRateFromPLN(targetCurrency);
            }
            else if (targetCurrency == "PLN")
            {
                return 1 / GetExchangeRateFromPLN(baseCurrency);
            }
            else
            {
                double rateToPLN = GetExchangeRateFromPLN(baseCurrency);
                double rateFromPLN = GetExchangeRateFromPLN(targetCurrency);
                return rateFromPLN / rateToPLN;
            }
        }

        private double GetExchangeRateFromPLN(string currency)
        {
            string url = $"https://api.nbp.pl/api/exchangerates/rates/a/{currency}/?format=json";
            string json;

            using (var webClient = new WebClient())
            {
                json = webClient.DownloadString(url);
            }

            Currency c = JsonSerializer.Deserialize<Currency>(json);
            return c.rates[0].mid ?? 0;
        }
    }
}