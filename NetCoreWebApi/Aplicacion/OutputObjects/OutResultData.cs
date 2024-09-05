using System;
using System.Collections.Generic;
using System.Text;

namespace OutputObjets

{
    public class OutResultData<T>
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public T data { get; set; }

        public OutResultData()
        {
            this.statusCode = 0;
            this.message = "";
        }
    }
}
