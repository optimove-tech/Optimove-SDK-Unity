using System;
using UnityEngine;
using System.Collections.Generic;

namespace OptimoveSdk
{
    public static class DictionaryExtension
    {
        public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }
    }

    public class PushMessage
    {
        public long Id { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public Dictionary<string, object> Data { get; private set; }
        public string Url { get; private set; }
        public string ActionId { get; private set; }

        public static PushMessage CreateFromJson(string message)
        {
            var data = MiniJSON.Json.Deserialize(message) as Dictionary<string, object>;

            if (data == null)
            {
                return null;
            }

            var push = new PushMessage();

            push.Id = (long)data["id"];
            push.Title = data.GetValueOrDefault("title") as string;
            push.Message = data.GetValueOrDefault("message") as string;
            push.Url = data.GetValueOrDefault("url") as string;
            push.Data = data.GetValueOrDefault("data") as Dictionary<string, object>;
            push.ActionId =  data.GetValueOrDefault("actionId") as string;

            return push;
        }
    }

    public class InAppInboxSummary
    {
        public uint TotalCount { get; private set; }
        public uint UnreadCount { get; private set; }

        internal static InAppInboxSummary CreateFromDictionary(Dictionary<string, object> dict)
        {
            var summary = new InAppInboxSummary();
            summary.TotalCount = Convert.ToUInt32((long) dict["totalCount"]);
            summary.UnreadCount = Convert.ToUInt32((long) dict["unreadCount"]);
            return summary;
        }
    }

    public class InAppButtonPress
    {
        public long MessageId { get; private set; }
        public Dictionary<string, object> DeepLinkData { get; private set; }
        public Dictionary<string, object> MessageData { get; private set; }

        internal static InAppButtonPress CreateFromJson(string json)
        {
            var data = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;

            var press = new InAppButtonPress();
            press.MessageId = (long) data["messageId"];
            press.DeepLinkData = data["deepLinkData"] as Dictionary<string, object>;
            press.MessageData = data.GetValueOrDefault("messageData") as Dictionary<string, object>;
            return press;
        }
    }

    public enum OptimoveInAppPresentationResult
    {
        presented,
        expired,
        failed
    }

    public class InAppInboxItem
    {
        public long Id { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public DateTime? AvailableFrom { get; private set; }
        public DateTime? AvailableTo { get; private set; }
        public DateTime? DismissedAt { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime SentAt { get; private set; }
        public Dictionary<string, object> Data { get; private set; }
        public string ImageUrl { get; private set; }

        public static List<InAppInboxItem> ListFromJson(string json)
        {
            var parsed = MiniJSON.Json.Deserialize(json) as List<object>;

            var items = new List<InAppInboxItem>();
            if (parsed == null)
            {
                return items;
            }

            foreach (var obj in parsed)
            {
                items.Add(CreateFromObj(obj));
            }

            return items;
        }

        private static InAppInboxItem CreateFromObj(object parsed)
        {
            var obj = parsed as Dictionary<string, object>;

            if (parsed == null || obj == null)
            {
                return null;
            }

            var item = new InAppInboxItem();

            item.Id = (long)obj["id"];
            item.Title = obj["title"] as string;
            item.Subtitle = obj["subtitle"] as string;
            item.AvailableFrom = InAppInboxItem.parseOptionalDate(obj.GetValueOrDefault("availableFrom") as string);
            item.AvailableTo = InAppInboxItem.parseOptionalDate(obj.GetValueOrDefault("availableTo") as string);
            item.DismissedAt = InAppInboxItem.parseOptionalDate(obj.GetValueOrDefault("dismissedAt") as string);
            item.IsRead = (bool)obj.GetValueOrDefault("isRead");
            item.SentAt = parseDate(obj["sentAt"] as string);
            item.Data = obj.GetValueOrDefault("data") as Dictionary<string, object>;
            item.ImageUrl = obj.GetValueOrDefault("imageUrl") as string;

            return item;
        }

		private static DateTime? parseOptionalDate(string date)
		{
			if (date == null){
				return null;
			}

			return parseDate(date);
		}

		private static DateTime parseDate(string date){
			return DateTime.Parse(date, null, System.Globalization.DateTimeStyles.RoundtripKind);
		}
    }
}
