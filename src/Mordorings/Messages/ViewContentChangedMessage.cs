using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Mordorings.Messages;

public class ViewContentChangedMessage(IViewModel value) : ValueChangedMessage<IViewModel>(value);
