//
// AmpacheSelectionFactory.cs
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
using System.Threading;
using JohnMoore.AmpacheNet.Entities;
using System.IO;
using System.Data.Sqlite;

namespace JohnMoore.AmpacheNet.DataAccess
{
    [Obsolete]
    public class AmpacheSelectionFactory
    {
        private Authenticate _handshake;
        private Jice.JiceContainer _container = new Jice.JiceContainer();
        public static string ArtLocalDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".AmpacheNet");
        public static string DatabaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string DbConnString { get { return string.Format("Data Source={0}", Path.Combine(DatabaseDirectory, "ampachenet.db3")); } }
		public virtual Handshake Handshake { get { return _handshake; } }

		public AmpacheSelectionFactory ()
		{
            DataAccess.Configurator.Configure(_container);
        }
		
        public AmpacheSelectionFactory (Authenticate hs) : this()
        {
            _handshake = hs;
            _container.Register<Handshake>().To(_handshake);
        }

        public virtual IAmpacheSelector<TEntity> GetInstanceSelectorFor<TEntity>() where TEntity : IEntity
        {
            try
            {
                return _container.Resolve<IAmpacheSelector<TEntity>>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format("{0} is not yet supported for selection from ampache", typeof(TEntity).Name), e);
            }
        }
		
		public virtual IPersister<TEntity> GetPersistorFor<TEntity>() where TEntity : IEntity
		{
            try
            {
                return _container.Resolve<IPersister<TEntity>>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format("{0} is not yet supported for persisting", typeof(TEntity).Name), e);
            }
		}
		
		public virtual Authenticate AuthenticateToServer(UserConfiguration config)
		{
			var tmp = new Authenticate(config.ServerUrl, config.User, config.Password);
            _handshake = tmp;
            _container.Register<Handshake>().To(_handshake);
			return tmp;
		}
		
		public virtual Authenticate AuthenticationTest(string server, string user, string password)
		{
			var tmp = new Authenticate(server, user, password);
			return tmp;
		}
		
		public virtual void Ping()
		{
			if(_handshake != null)
			{
				_handshake.Ping();
			}
		}
    }
}
