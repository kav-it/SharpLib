// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using NLog.Config;
using NLog.Internal;

#if !SILVERLIGHT

namespace NLog.LayoutRenderers
{
    /// <summary>
    /// ASP Request variable.
    /// </summary>
    [LayoutRenderer("asp-request")]
    public class AspRequestValueLayoutRenderer : LayoutRenderer
    {
        #region ��������

        /// <summary>
        /// Gets or sets the item name. The QueryString, Form, Cookies, or ServerVariables collection variables having the
        /// specified name are rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the QueryString variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the form variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Form { get; set; }

        /// <summary>
        /// Gets or sets the cookie to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Cookie { get; set; }

        /// <summary>
        /// Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string ServerVariable { get; set; }

        #endregion

        #region ������

        /// <summary>
        /// Renders the specified ASP Request variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AspHelper.IRequest request = AspHelper.GetRequestObject();
            if (request != null)
            {
                if (QueryString != null)
                {
                    builder.Append(GetItem(request.GetQueryString(), QueryString));
                }
                else if (Form != null)
                {
                    builder.Append(GetItem(request.GetForm(), Form));
                }
                else if (Cookie != null)
                {
                    object cookie = request.GetCookies().GetItem(Cookie);
                    builder.Append(Convert.ToString(AspHelper.GetComDefaultProperty(cookie), CultureInfo.InvariantCulture));
                }
                else if (ServerVariable != null)
                {
                    builder.Append(GetItem(request.GetServerVariables(), ServerVariable));
                }
                else if (Item != null)
                {
                    AspHelper.IDispatch o = request.GetItem(Item);
                    AspHelper.IStringList sl = o as AspHelper.IStringList;
                    if (sl != null)
                    {
                        if (sl.GetCount() > 0)
                        {
                            builder.Append(sl.GetItem(1));
                        }

                        Marshal.ReleaseComObject(sl);
                    }
                }

                Marshal.ReleaseComObject(request);
            }
        }

        private static string GetItem(AspHelper.IRequestDictionary dict, string key)
        {
            object retVal = null;
            object o = dict.GetItem(key);
            AspHelper.IStringList sl = o as AspHelper.IStringList;
            if (sl != null)
            {
                if (sl.GetCount() > 0)
                {
                    retVal = sl.GetItem(1);
                }

                Marshal.ReleaseComObject(sl);
            }
            else
            {
                return o.GetType().ToString();
            }

            return Convert.ToString(retVal, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}

#endif