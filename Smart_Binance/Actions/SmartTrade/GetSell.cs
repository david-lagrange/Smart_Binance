﻿using Binance.Net;
using Binance.Net.Objects;
using Smart_Binance.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_Binance.Actions.SmartTrade
{
    public class GetSell
    {
        public async Task<TokenViewModel> Info(string market)
        {

            TokenViewModel viewModel = new TokenViewModel();
            viewModel.BaseType = BaseType(market);
            string asset = market.Replace(viewModel.BaseType, "");
            using (var client = new BinanceClient())
            {
                var balances = await client.GetAccountInfoAsync();
                if (balances.Success)
                {
                    BinanceBalance balance = balances.Data.Balances.Where(b => b.Asset == asset).Single();
                    var currentPrice = await client.Get24HPriceAsync(market);
                    if (currentPrice.Success)
                    {
                        CalculateAmountDecimal amountDecimal = new CalculateAmountDecimal();
                        viewModel.DecimalAmount = await amountDecimal.OrderBookDecimal(market);
                        viewModel.Name = market;
                        viewModel.Amount = decimal.Round(balance.Free, viewModel.DecimalAmount);
                        viewModel.LastPrice = currentPrice.Data.LastPrice;
                        return viewModel;
                    }
                }
            }
            return viewModel;
        }
        private string BaseType(string market)
        {
            string end = market.Substring(market.Length - 4);
            end = end.Contains("BNB") ? "BNB" : end;
            end = end.Contains("BTC") ? "BTC" : end;
            end = end.Contains("USDT") ? "USDT" : end;
            end = end.Contains("ETH") ? "ETH" : end;
            end = end.Contains("XRP") ? "XRP" : end;
            return end;
        }
    }
}