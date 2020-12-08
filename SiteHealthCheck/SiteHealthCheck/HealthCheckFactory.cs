using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteHealthCheck
{
    public class HealthCheckFactory
    {
        private bool IsWorking { get; set; } = false;
        private bool Success { get; set; } = true;
        private DateTime LastCheck { get; set; } = DateTime.MinValue;
        public string URL { get; set; }
        public int Interval { get; set; }
        public bool UseHEADInsteadOfGET { get; set; }
        public static HealthCheckFactory Default { get; set; } 


        public HealthCheckFactory(string url, int interval, bool useHEADInsteadOfGET = true)
        {
            this.URL = url.Trim().ToLower();
            this.Interval = interval;
            this.UseHEADInsteadOfGET = useHEADInsteadOfGET;
        }

        public bool GetSuccess()
        {
            if (LastCheck < DateTime.Now.AddSeconds(0 - Interval))
            {
                if (!IsWorking)
                {
                    IsWorking = true;
                    Task.Factory.StartNew(() => CheckSite());
                }
            }
            return Success;
        }

        private async void CheckSite()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    if (this.UseHEADInsteadOfGET)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, this.URL);
                        hc.DefaultRequestHeaders.Accept.Clear();
                        var r = await hc.SendAsync(request);
                        this.Success = r.IsSuccessStatusCode;
                    }
                    else
                    {
                        var r = await hc.GetAsync(this.URL);
                        this.Success = r.IsSuccessStatusCode;
                    }
                }
            }
            catch
            {
                this.Success = false;
            }
            finally
            {
                this.IsWorking = false;
                this.LastCheck = DateTime.Now;
            }
        }
    }
}
