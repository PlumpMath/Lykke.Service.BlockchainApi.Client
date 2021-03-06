﻿using System;
using JetBrains.Annotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;

namespace Lykke.Service.BlockchainApi.Client.Models
{
    [PublicAPI]
    public class HistoricalTransaction
    {
        /// <summary>
        /// Lykke unique operation ID
        /// </summary>
        public Guid OperationId { get; }

        /// <summary>
        /// Transaction moment in UTC
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Source address
        /// </summary>
        public string FromAddress { get; }

        /// <summary>
        /// Destination address
        /// </summary>
        public string ToAddress { get; }

        /// <summary>
        /// Asset ID
        /// </summary>
        public string AssetId { get; }

        /// <summary>
        /// Amount without fee
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Transaction hash as base64 string.
        /// </summary>
        public string Hash { get; }

        public HistoricalTransaction(HistoricalTransactionContract contract, int assetAccuracy)
        {
            if (contract == null)
            {
                throw new ResultValidationException("Contract value is required");
            }
            if (contract.OperationId == Guid.Empty)
            {
                throw new ResultValidationException("Operation ID is required", contract.OperationId);
            }
            if (contract.Timestamp == DateTime.MinValue)
            {
                throw new ResultValidationException("Timestamp is required", contract.Timestamp);
            }
            if (contract.Timestamp.Kind != DateTimeKind.Utc)
            {
                throw new ResultValidationException("Timestamp kind should be UTC", contract.Timestamp.Kind);
            }
            if (string.IsNullOrWhiteSpace(contract.FromAddress))
            {
                throw new ResultValidationException("Source address is required", contract.FromAddress);
            }
            if (string.IsNullOrWhiteSpace(contract.ToAddress))
            {
                throw new ResultValidationException("Destination address is required", contract.ToAddress);
            }
            if (string.IsNullOrWhiteSpace(contract.AssetId))
            {
                throw new ResultValidationException("Asset ID is required", contract.AssetId);
            }
            if (string.IsNullOrWhiteSpace(contract.Hash))
            {
                throw new ResultValidationException("Hash is required", contract.Hash);
            }

            try
            {
                Amount = Conversions.CoinsFromContract(contract.Amount, assetAccuracy);

                if (Amount <= 0)
                {
                    throw new ResultValidationException("Amount should be positive number", contract.Amount);
                }
            }
            catch (ConversionException ex)
            {
                throw new ResultValidationException("Failed to parse amount", contract.Amount, ex);
            }

            OperationId = contract.OperationId;
            Timestamp = contract.Timestamp;
            FromAddress = contract.FromAddress;
            ToAddress = contract.ToAddress;
            AssetId = contract.AssetId;
            Hash = contract.Hash;
        }
    }
}
