﻿using Backendt1.DataObjects;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Backendt1.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class VictimController : ApiController
    {
        // GET: api/Victim
        public ArrayList Get()
        {
            VictimPersistence pp = new VictimPersistence();
            return pp.getVictims();
        }

        // GET: api/Victim/5
        public Victim Get(String id)
        {
            VictimPersistence pp = new VictimPersistence();
            Victim p = pp.getVictim(id);
            if (p == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return p;
        }

        // POST: api/Victim
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post([FromBody]Victim value)
        {

            VictimPersistence pp = new VictimPersistence();
            String id;
            id = pp.saveUser(value);

            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = value.UserName + " Need Help from you. The User ID =" + value.FBID;

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }

            value.FBID = id;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/user/{0}", id));
            return response;

        }

        // PUT: api/Victim/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Victim/5
        public void Delete(int id)
        {


        }
    }
}
