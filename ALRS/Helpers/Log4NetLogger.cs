using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
using log4net;
using log4net.Core;

namespace ALRSSystem.Helpers
{
     public class Log4NetLogger : ILogger
     {
 
        //private ILog _logger;

        private  readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

 
         public Log4NetLogger()
         {
            //_logger = LogManager.GetLogger(this.GetType());
             _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
         }
 
         public void Info(string message)
         {
            _logger.Info(message);
         }
 
         public void Warn(string message)
         {
            _logger.Warn(message);
         }
 
         public void Debug(string message)
         {
            _logger.Debug(message);
         }
 
         public void Error(string message)
         {
         _logger.Error(message);
         }
 
         public void Error(Exception x)
         {
             Error(new LogException("Error:", x));
             //Error(LogUtility.BuildExceptionMessage(x));
         }
 
         public void Error(string message, Exception x)
         {
            _logger.Error(message, x);
         }
 
         public void Fatal(string message)
         {
            _logger.Fatal(message);
         }
 
         public void Fatal(Exception x)
         {
             Fatal(new LogException("Fatal:", x));
         }


         public bool IsEnabledFor(Level level)
         {
             throw new NotImplementedException();
         }

         public void Log(LoggingEvent logEvent)
         {
             throw new NotImplementedException();
         }

         public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
         {
             throw new NotImplementedException();
         }

         public string Name
         {
             get { throw new NotImplementedException(); }
         }

         public log4net.Repository.ILoggerRepository Repository
         {
             get { throw new NotImplementedException(); }
         }
     }
}