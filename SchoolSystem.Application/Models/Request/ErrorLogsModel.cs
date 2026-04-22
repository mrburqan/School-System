using System;

namespace SchoolSystem.Application.Models.Request
{
    public class ErrorLogsModel
    {
        public int ErrorId { get; set; }
        private string _errorCode;
        public string ErrorCode
        {
            get => _errorCode;
            set => _errorCode = value?.Trim();
        }

        private string _userMessage;
        public string UserMessage
        {
            get => _userMessage;
            set => _userMessage = value?.Trim();
        }

        private string _technicalMessage;
        public string TechnicalMessage
        {
            get => _technicalMessage;
            set => _technicalMessage = value?.Trim();
        }

        private string _stackTraces;
        public string StackTraces
        {
            get => _stackTraces;
            set => _stackTraces = value?.Trim();
        }

        private string _source;
        public string Source
        {
            get => _source;
            set => _source = value?.Trim();
        }

        private string _userId;
        public string UserID
        {
            get => _userId;
            set => _userId = value?.Trim();
        }
        public DateTime? CreatedAt { get; set; }

        public bool IsResolve { get; set; }
        public DateTime? ResolvedAt { get; set; }
        private string _resolutionNote;
        public string ResolutionNote
        {
            get => _resolutionNote;
            set => _resolutionNote = value?.Trim();
        }
    }
}