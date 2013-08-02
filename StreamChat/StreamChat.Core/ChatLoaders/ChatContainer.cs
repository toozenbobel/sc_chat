using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using StreamChat.Core.Chats;
using StreamChat.Core.MvxMessages;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public interface IChatContainer
	{
		void AddChat(long sourceId, string chatUri);
		IEnumerable<IChat> GetChats();
	}

	public class ChatContainer : IChatContainer
	{
		public class StoreChat : IChat
		{
			public long SourceId { get; set; }
			public string ServiceName { get; private set; }
			public string ChatUri { get; set; }
			public IChatLoadingService ChatLoader { get; private set; }
			public string StreamerNick { get; set; }
			public string StreamerId { get; set; }
		}

		private readonly IChatResolveService _resolveService;
		private readonly IMvxMessenger _messenger;
		private readonly IMvxFileStore _fileService;

		public ChatContainer(IChatResolveService resolveService, IMvxMessenger messenger, IMvxFileStore fileService)
		{
			_resolveService = resolveService;
			_messenger = messenger;
			_fileService = fileService;
		}

		private List<IChat> _chats = new List<IChat>();

		public IEnumerable<IChat> GetChats()
		{
			if (!_chats.Any())
			{
				var loadedFromFile = TryLoadFromFile();
				if (loadedFromFile != null)
					_chats = loadedFromFile.ToList();
			}

			return _chats;
		}

		public void AddChat(long sourceId, string chatUri)
		{
			IChat newChat = _resolveService.CreateNewInstance(sourceId);
			newChat.ChatUri = chatUri;
			_chats.Add(newChat);
			
			_messenger.Publish(new ChatAddedMessage(this));
			UpdateFileStore();
		}

		#region Update and load from file

		private const string FILENAME = "chats.json";

		private void UpdateFileStore()
		{
			_fileService.WriteFile(FILENAME, stream =>
				{
					using (var writer = new StreamWriter(stream))
					{
						var jsonString = JsonConvert.SerializeObject(_chats);
						writer.Write(jsonString);
					}
				});
		}

		private IEnumerable<IChat> TryLoadFromFile()
		{
			var convertedChats = new List<IChat>();

			var loadedChats = new List<StoreChat>();
			_fileService.TryReadBinaryFile(FILENAME, stream =>
			{
				try
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						var fileContent = reader.ReadToEnd();
						loadedChats = JsonConvert.DeserializeObject<List<StoreChat>>(fileContent);

						convertedChats = loadedChats.Select(lc =>
							{
								IChat ret = _resolveService.CreateNewInstance(lc.SourceId);
								ret.ChatUri = lc.ChatUri;
								return ret;
							}).ToList();

						return true;
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine("Problem loading chat data from file. {0}", e.ToLongString());
					return false;
				}
			});

			return convertedChats;
		}

		#endregion

		
	}
}
