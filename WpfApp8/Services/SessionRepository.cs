using PCClubAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PCClubAdmin.Services
{
    public class SessionRepository
    {
        public List<Session> GetAll()
        {
            return ApiClient.Get<List<Session>>("api/sessions");
        }

        public List<Session> GetByPeriod(DateTime start, DateTime end, string computerName)
        {
            var path = "api/sessions";
            if (!string.IsNullOrWhiteSpace(computerName))
            {
                path += "?computer=" + Uri.EscapeDataString(computerName);
            }

            return ApiClient.Get<List<Session>>(path)
                .Where(session => session.StartTime >= start && session.StartTime <= end)
                .ToList();
        }

        public int Add(Session session)
        {
            var request = new StartSessionRequest
            {
                ClientName = session.ClientName,
                ComputerName = session.ComputerName,
                TariffName = session.TariffName,
                HourlyRate = session.HourlyRate ?? 0m
            };

            var created = ApiClient.Post<Session>("api/sessions", request);
            session.StartTime = created.StartTime;
            session.EndTime = created.EndTime;
            session.TotalCost = created.TotalCost;
            return created.Id;
        }

        public void Complete(Session session)
        {
            var completed = ApiClient.Post<Session>($"api/sessions/{session.Id}/complete", new { });
            session.EndTime = completed.EndTime;
            session.TotalCost = completed.TotalCost;
        }

        public void Delete(int id)
        {
            ApiClient.Delete($"api/sessions/{id}");
        }

        private class StartSessionRequest
        {
            public string ClientName { get; set; }
            public string ComputerName { get; set; }
            public string TariffName { get; set; }
            public decimal HourlyRate { get; set; }
        }
    }
}
