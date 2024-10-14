using System;

namespace DDD.Domain.Exceptions
{
    /// <summary>
    /// 例外基底クラス
    /// </summary>
    public abstract class ExceptionBase : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public ExceptionBase(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="exception">元になった例外</param>
        public ExceptionBase(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 例外区分
        /// </summary>
        public abstract ExceptionKind Kind { get; }

        /// <summary>
        /// 例外区分
        /// </summary>
        public enum ExceptionKind
        {
            Info,
            Warning,
            Error,
        }
    }
}
