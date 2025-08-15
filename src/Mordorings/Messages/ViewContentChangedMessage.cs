using CommunityToolkit.Mvvm.Messaging.Messages;
using Mordorings.ViewModels.Abstractions;

namespace Mordorings.Messages;

public class ViewContentChangedMessage(IViewModel value) : ValueChangedMessage<IViewModel>(value);
