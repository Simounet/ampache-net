//
// AmpacheSelectorBase.cs
//
// Author:
//       John Moore <jcwmoore@gmail.com>
//
// Copyright (c) 2010 John Moore
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using JohnMoore.AmpacheNet.Entities;

namespace JohnMoore.AmpacheNet.DataAccess
{
    internal abstract class AmpacheSelectorBase<TEntity> : IAmpacheSelector<TEntity> where TEntity : IEntity
    {
        protected const string BASE_URL = @"{0}/server/xml.server.php?action={1}&auth={2}";
        protected const string FILTER_PARAMETER = @"&filter={0}";
        protected const string OFFSET_PARAMETER = @"&offset={0}";
        protected const string LIMIT_PARAMETER = @"&limit={0}";
        private const int LIMIT_AMOUNT = 500;
        protected readonly Handshake _handshake;
        private readonly IEntityFactory<TEntity> _factory;
        protected abstract string SelectAllMethod { get; }
        protected abstract string XmlNodeName { get; }
		protected abstract string SelectSingleMethod { get; }
        protected abstract Dictionary<Type, string> SelectMethodMap { get; }

        public AmpacheSelectorBase (Handshake handshake, IEntityFactory<TEntity> factory)
        {
            _handshake = handshake;
            _factory = factory;
        }

        #region IAmpacheSelector[TEntity] implementation

        public virtual IEnumerable<TEntity> SelectAll ()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(BASE_URL, _handshake.Server, SelectAllMethod, _handshake.Passphrase);
            builder.AppendFormat(LIMIT_PARAMETER, LIMIT_AMOUNT);
            var tmp = Query(builder.ToString());
            int offset = 0;
			var results = new List<TEntity>();
            while (tmp.Count == LIMIT_AMOUNT) {
                results.AddRange(tmp);
                offset += LIMIT_AMOUNT;
                builder = new StringBuilder();
                builder.AppendFormat(BASE_URL, _handshake.Server, SelectAllMethod, _handshake.Passphrase);
                builder.AppendFormat(LIMIT_PARAMETER, LIMIT_AMOUNT);
                builder.AppendFormat(OFFSET_PARAMETER, offset);
                tmp = Query(builder.ToString());
            }
			results.AddRange(tmp);
            return results;		
        }

        public virtual IEnumerable<TEntity> SelectBy<TParameter> (TParameter parameter) where TParameter : IEntity
        {
			if(parameter is TEntity)
			{
				return new TEntity[] {(TEntity)(IEntity)parameter};
			}
            if (!SelectMethodMap.ContainsKey(typeof(TParameter)))
            {
                return SelectAll();
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(BASE_URL, _handshake.Server, SelectMethodMap[typeof(TParameter)], _handshake.Passphrase);
            builder.AppendFormat(FILTER_PARAMETER, parameter.Id);
            return Query(builder.ToString());
        }

		public IEnumerable<TEntity> SelectBy(string searchText)
		{
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(BASE_URL, _handshake.Server, SelectAllMethod, _handshake.Passphrase);
            builder.AppendFormat(FILTER_PARAMETER, searchText);
			builder.AppendFormat(LIMIT_PARAMETER, 500);
            return Query(builder.ToString());
		}

		public TEntity SelectBy(int ampacheId)
		{
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(BASE_URL, _handshake.Server, SelectSingleMethod, _handshake.Passphrase);
            builder.AppendFormat(FILTER_PARAMETER, ampacheId);
            return Query(builder.ToString()).FirstOrDefault();
		}
		
        #endregion
		
        private ICollection<TEntity> Query(string url)
        {
            var request = WebRequest.Create (url);
            var response = request.GetResponse();
			try
			{
            	var result = XElement.Load(new StreamReader(response.GetResponseStream()));
                return _factory.Construct(result.Descendants(XmlNodeName).ToList());
			}
			catch(Exception e)
			{
				Console.WriteLine ("Error in Response or no results");
				Console.WriteLine (e.Message);
				return new List<TEntity>();
			}
        }
    }
}
