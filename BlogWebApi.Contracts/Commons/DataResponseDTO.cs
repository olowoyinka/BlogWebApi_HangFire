using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace BlogWebApi.Contracts.Commons
{
    public abstract class DataResponseAbstractDTO
    {
        public HttpStatusCode status { get; set; }
        public bool successful { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class DataResponseArrayDTO<T> : DataResponseAbstractDTO
    {
        /// <summary>
        /// The items being returned
        /// </summary>
        public IEnumerable<T> Data { get; set; }
        /// <summary>
        /// The page of items being returned
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// The size of items in each page
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// The total number of items to be returned
        /// </summary>
        public int TotalNumber { get; set; }

        public DataResponseArrayDTO(IEnumerable<T> items,
                                    int totalNumber,
                                    int page = 1,
                                    int size = 20,
                                    HttpStatusCode statusCode = HttpStatusCode.OK,
                                    bool isSuccess = true)
        {
            Data = items;
            Page = page;
            status = statusCode;
            successful = isSuccess;
            Size = size < 1 ? 20 : size;
            TotalNumber = totalNumber;// < items.Count() ? items.Count() : count;

            Size = Size > TotalNumber ? TotalNumber : Size;
        }
    }

    public class DataResponseListDTO<T> : DataResponseAbstractDTO
    {
        /// <summary>
        /// The items being returned
        /// </summary>
        public IEnumerable<T> data { get; set; }

        public DataResponseListDTO(IEnumerable<T> items,
                                    HttpStatusCode statusCode = HttpStatusCode.OK,
                                    bool isSuccess = true)
        {
            data = items;
            status = statusCode;
            successful = isSuccess;
        }
    }

    public class DataResponseDTO<T> : DataResponseAbstractDTO
    {
        public T data { get; set; }

        public DataResponseDTO(T dataItem,
                               HttpStatusCode statusCode = HttpStatusCode.OK,
                               bool isSuccess = true)
        {
            data = dataItem;
            status = statusCode;
            successful = isSuccess;
        }
    }

    public class ErrorResponseDTO : DataResponseAbstractDTO
    {
        public IEnumerable<string> ErrorMessages { get; set; }

        public ErrorResponseDTO(HttpStatusCode statusCode, IEnumerable<string> errors)
        {
            status = statusCode;
            ErrorMessages = errors;
            successful = false;
        }
    }

    public class ModelStateErrorResponseDTO : DataResponseAbstractDTO
    {
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public ModelStateErrorResponseDTO(HttpStatusCode statusCode, ModelStateDictionary modelStateErrors)
        {
            status = statusCode;
            ErrorMessages.AddRange(modelStateErrors.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            successful = false;
        }
    }

    /// <summary>
	/// Limits a model string property to a predefined set of values
	/// </summary>
	public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }
}
