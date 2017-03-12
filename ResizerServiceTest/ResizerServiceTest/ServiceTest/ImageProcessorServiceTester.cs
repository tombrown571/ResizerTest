using ResizerServiceTest.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.IO;
using System.Net.Http;

namespace ResizerServiceTest.ServiceTest
{
    public class ImageProcessorServiceTester : IImageServiceTester
    {
        public ConcurrentBag<ErrorLogStruct> Exceptions { get; }

        private const string baseUrl = "http://localhost:5000";

        private string _requestUrl;

        public async System.Threading.Tasks.Task<bool> GetRequestAsync()
        {
            bool finishedReq = false;
            HttpResponseMessage response = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    response = await client.GetAsync(_requestUrl);
                    response.EnsureSuccessStatusCode();
                    finishedReq = (int)response.StatusCode < 400;
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(new ErrorLogStruct()
                {
                    Ex = ex,
                    LogText = $"Status: {response?.StatusCode:-1} Request: {_requestUrl}"
                });
            }
            return finishedReq;
        }


        public bool GetRequest()
        {
            bool finishedReq = false;
            HttpResponseMessage response = null;
            try
            {
                var client = new HttpClient();
                var task = client.GetAsync(_requestUrl);
                response = task.Result;
                finishedReq = (int)response.StatusCode < 400;
            }
            catch (Exception ex)
            {
                Exceptions.Add(new ErrorLogStruct()
                {
                    Ex = ex,
                    LogText = $"Status: {response?.StatusCode:-1} Request: {_requestUrl}"
                });
            }
            return finishedReq;
        }

        private ImageProcessorServiceTester()
        {
            Exceptions = new ConcurrentBag<ErrorLogStruct>();
        }
        public ImageProcessorServiceTester(string imageName) : this()
        {
            _requestUrl = $"{baseUrl}/images/{imageName}";
        }

        public ImageProcessorServiceTester(string presetName, string imageName) : this()
        {
            _requestUrl = $"{baseUrl}/images/{presetName}/{imageName}";
        }
    }
}
