### What is WFBind?

Simply put, WFBind is a library allowing your Windows Forms applications to make use of the MVVM pattern, similar to what is baked into WPF.

### But why?

Inspiration for this project came from a real project - legacy code base in need of addition of a new application. Proposed WPF solution was shot down as too unpredictable and consistency with existing code base was required.

Still we wanted to go the MVVM way, to make the code cleaner and more readable/maintainable. So we decided to roll our own MVVM for WinForms. And so this project was inspired by the code we produced, while in no way connected to the original.

### So what can I do with WFBind?

Right now, the answer to that question is "very little". WFBind is currently in its infancy, if that, so there is much more stuff in the "planned" category than in the "done" category. What you can do, though, is to look at the existing code, clone it, and play around with it. Or you could even contribute, if you feel so inclined!

To make this perfectly clear: WFBind does not contain a library of new, bindable UI controls. It only works with the existing WinForms controls and adds a way to bind them to view models, including command bindings and change notification.

### Roadmap

So what's on the agenda for WFBind? Let's see...

Done and/or prototyped:
* Basic architecture for binding Forms and UserControls to view models
* Two-way bindings for simple components:
    * TextBox
    * CheckBox
    * RadioButton
* Command bindings
    * Button
    * ToolStripButton
    * MenuItem
    * ToolStripMenuItem

Still to come:
* Bindings to collection controls:
    * ComboBox
    * ListView
    * Listbox
    * DataGridView
* Command bindings
    * whatever else that comes to mind
* Binding converters (one- and two-way)
* Binding templating
* Any other UI component binding I'll think of

The whole code base will also be covered by unit tests and will require some major refactorings as it moves forward.
