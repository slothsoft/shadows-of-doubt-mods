using System;
using System.Linq;
using LiveYourLife.Common;
using SOD.Common.Extensions;
using UnityEngine;

namespace LiveYourLife.AddressMoves;

internal static class MovingNotice
{
    internal const string TreeId = "61851517-4896-47a5-ad59-46af1fb9cb17";
    internal const string MessageId = "LiveYourLife.MovingNotice.Body";
    private const string SourceTreeId = "970c4114-def0-4e04-8982-da36e01f4905"; // existing password note as base
    // A mod-specific numeric value avoids triggering the game's built-in PassedVar handlers.
    private const Interactable.PassedVarType TextVariable = (Interactable.PassedVarType)1739501;

    internal static bool RegisterDocument()
    {
        var trees = Toolbox.Instance.allDDSTrees;
        if (trees.ContainsKey(TreeId))
            return true;

        if (!trees.TryGetValue(SourceTreeId, out var source))
            return false;

        var sourceMessage = FindBodyLayout(source);
        if (sourceMessage == null || source.document == null)
            return false;

        var message = CopyBodyLayout(sourceMessage);
        var tree = new DDSSaveClasses.DDSTreeSave
        {
            id = TreeId,
            name = "LiveYourLife moving notice",
            treeType = source.treeType,
            document = source.document,
            startingMessage = message.instanceID,
        };
        tree.messages.Add(message);
        trees.Add(TreeId, tree);
        return true;
    }

    private static DDSSaveClasses.DDSMessageSettings? FindBodyLayout(DDSSaveClasses.DDSTreeSave tree)
    {
        foreach (var message in tree.messages)
        {
            if (!string.IsNullOrEmpty(message.msgID))
                return message;
        }
        return null;
    }

    private static DDSSaveClasses.DDSMessageSettings CopyBodyLayout(DDSSaveClasses.DDSMessageSettings source)
    {
        return new DDSSaveClasses.DDSMessageSettings
        {
            msgID = MessageId,
            instanceID = "LiveYourLife.MovingNotice.BodyLayout",
            elementName = "Moving notice",
            pos = source.pos,
            size = source.size,
            rot = source.rot,
            font = source.font,
            col = source.col,
            fontSize = source.fontSize,
            charSpace = source.charSpace,
            wordSpace = source.wordSpace,
            lineSpace = source.lineSpace,
            paraSpace = source.paraSpace,
            alignH = source.alignH,
            alignV = source.alignV,
            fontStyle = source.fontStyle,
            usePages = source.usePages,
            isHandwriting = source.isHandwriting,
        };
    }

    internal static string CaptureText(NewAddress oldAddress, NewAddress newAddress, Human[] movers)
    {
        var names = string.Join("\n", movers.Select(GetCitizenName));
        // Capture strings now: later moves must not change an existing document.
        return "CHANGE OF ADDRESS\n\n" + names +
               "\n\nPrevious address:\n" + oldAddress.name +
               "\n\nPlease forward mail to:\n" + newAddress.name +
               "\n\nMoved on: " + SessionData.Instance.gameTime.AsGameTimeString();
    }

    private static string GetCitizenName(Human human)
    {
        return human.GetCitizenName();
    }

    internal static void Place(NewAddress oldAddress, Human writer, string text, Action<string> log)
    {
        if (!RegisterDocument())
        {
            log("Moving notice skipped: DDS document layout unavailable.");
            return;
        }

        var node = FindEntranceNode(oldAddress);
        if (node == null)
        {
            log($"Moving notice skipped: no placement node at {oldAddress.name}.");
            return;
        }

        // World objects survive removal of the old apartment's furniture.
        // Passed variables and the DDS override are serialized by the game.
        var variables = new Il2CppSystem.Collections.Generic.List<Interactable.Passed>();
        variables.Add(new Interactable.Passed(TextVariable, 0f, text));
        var note = InteractableCreator.Instance.CreateWorldInteractable(
            InteriorControls.Instance.note, 
            writer, 
            writer, 
            writer,
            node.position + new Vector3(0f, 0.06f, 0f),
            new Vector3(0f, 90f, 0f), 
            variables, 
            null, 
            TreeId);
        
        if (note == null)
        {
            log($"Moving notice could not be created at {oldAddress.name}.");
            return;
        }

        note.evidence?.AddOrSetCustomName(Evidence.DataKey.name, "Change of address");
        log($"Moving notice created at {oldAddress.name}: id={note.id}");
    }
    
    private static NewNode? FindEntranceNode(NewAddress address)
    {
        var entrance = address.GetMainEntrance();
        if (entrance == null)
            return address.anchorNode;

        if (entrance.fromNode.gameLocation == address)
            return entrance.toNode;

        if (entrance.toNode.gameLocation == address)
            return entrance.fromNode;

        return address.anchorNode;
    }
    
    internal static string? ReadText(Interactable? note)
    {
        if (note == null || note.pv == null)
            return null;

        foreach (var variable in note.pv)
        {
            if (variable.varType == TextVariable)
                return variable.str;
        }
        return null;
    }
}
