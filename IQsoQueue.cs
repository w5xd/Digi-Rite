/* the differences between contest and casual exchanges
 * is handled by classes that both comply with these interfaces. */

namespace WriteLogDigiRite
{
    public delegate void IsConversationMessage(Conversation.Origin reason);

    public interface IQsoQueue
    {
        string MyCall { set; }
        string MyBaseCall { set; }
        void OnCycleBeginning(int cycleNumber);
        bool InitiateQso(RecentMessage rm, short band, bool onHisFrequency, System.Action onUsed=null);
        void MessageForMycall(RecentMessage recentMessage,
                    bool directlyToMe, string callQsled, short band,
                    bool autoStart, IsConversationMessage onUsed);
   }

    public interface IQsoSequencer
    {
        bool OnReceivedNothing(); // return true on sent a message
        bool IsFinished { get; }
        string DisplayState { get; }
    }

}
