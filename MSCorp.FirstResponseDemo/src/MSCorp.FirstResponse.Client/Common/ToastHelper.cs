using System;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace MSCorp.FirstResponse.Client.Common
{
    public static class ToastHelper
    {
        public static ToastNotification PopIncidentUpdatedToast(int incidentId)
        {
            string xml = $@"<toast activationType='foreground' launch='{Constants.Incident}:{incidentId}'>
                            <visual>
                                <binding template='ToastGeneric'>
                                    <text>Dispatch updated incident</text>
                                </binding>
                            </visual>
                        </toast>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            
            var toast = new ToastNotification(doc);

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            return toast;
        }

        public static int GetIncidentNumber(string incidentArgKeyPair)
        {
            string[] pairArray = incidentArgKeyPair.Split(':');
            string key = pairArray[0];
            if (key != Constants.Incident || pairArray.Length != 2)
            {
                throw new ArgumentException($"incident key pair expected to be in the form: '{Constants.Incident}:[number]' but was in the form: {incidentArgKeyPair}");
            }

            try
            {
                return int.Parse(pairArray[1]);
            }
            catch (FormatException e)
            {
                throw new Exception("Incident value was not an int", e);
            }
        }
    }
}
