using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.OverlayModalService
{
    public class OverlayModalResult
    {
        public object Data { get; set; }
        public Type DataType { get; set; }
        public bool Cancelled { get; set; }
        public Type ModalType { get; set; }
        protected OverlayModalResult(object data, Type resultType, bool cancelled, Type modalType)
        {
            Data = data;
            DataType = resultType;
            Cancelled = cancelled;
            ModalType = modalType;
        }

        public static OverlayModalResult Ok<T>(T result) => Ok(result, default);
        public static OverlayModalResult Ok<T>(T result, Type modalType) => new OverlayModalResult(result, typeof(T), false, modalType);
        public static OverlayModalResult Cancel() => Cancel(default);
        public static OverlayModalResult Cancel(Type modelType) => new OverlayModalResult(default, typeof(object), true, modelType);
    }
}
