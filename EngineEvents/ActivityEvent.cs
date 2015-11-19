using System;
//using Drallo.ChallengeEngine.Attempt;
//using Drallo.ChallengeEngine.Feedback;

namespace WebsocketTest
{

		public enum ActivityEventType 
		{
			QRCodeScan, TextEntry, UserImage
		}

		public class ActivityEvent
		{
			public Guid Id { get; set; }

			public DateTime Timestamp { get; set; }

			public ActivityEventType EventType { get; set;}

			//public QRCodeScan UserQRCodeScan { get; set; }
			//public TextEntry UserText { get; set; }
			//public Image UserImage { get; set; }

			public ActivityEvent()
			{
				this.Id = Guid.NewGuid();
			}

//			public override string ToString ()
//			{
//				return string.Format ("[ActivityRecord: Id={0}, Timestamp={1},UserQRCodeScan={2}, UserText={3}, UserImage={4}]", Id, Timestamp, UserQRCodeScan, UserText, UserImage);
//			}

//			public TimeLineEntry ToTimeLineEntry()
//			{
//				var feedbackItem = new FeedbackItem () { FeedbackType = FeedbackType.UserEntry };
//				var entry = new TimeLineEntry (feedbackItem);
//				switch(EventType)
//				{
//				case ActivityEventType.TextEntry:
//					feedbackItem.ContentText = UserText.Text;
//					break;
//				case ActivityEventType.UserImage:
//					feedbackItem.ContentText = "[Photo taken]";
//					break;
//				case ActivityEventType.QRCodeScan:
//					feedbackItem.ContentText = "[QR-Code scanned]";
//					break;
//				}
//				return entry;
//			}

		}
	}