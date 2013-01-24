using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    public class ServerUpdateException : Exception
    {
        #region Constants and Fields

        private readonly string _message;

        #endregion

        #region Constructors and Destructors

        public ServerUpdateException(string message)
        {
            _message = message;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get { return String.Format("Error Updatind Data: {0}", _message); }
        }

        #endregion
    }
}