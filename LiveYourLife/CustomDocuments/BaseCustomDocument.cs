using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using LiveYourLife.AddressMoves;
using UnityEngine;

namespace LiveYourLife.CustomDocuments;

public abstract class BaseCustomDocument
{
    public static (int registered, int failed) RegisterTypes()
    {
        var registered = 0;
        var failed = 0;
        foreach (var allType in AllTypes)
        {
            if (allType.RegisterType())
            {
                registered++;
            }
            else
            {
                failed++;
            }
        }
        return (registered, failed);
    }

    public static System.Collections.Generic.IEnumerable<BaseCustomDocument> AllTypes { get; } = CreateAllTypes().ToArray();
    
    private static System.Collections.Generic.IEnumerable<BaseCustomDocument> CreateAllTypes()
    {
        yield return new MovingNotice();
    }

    private const string SourceType_Password = "970c4114-def0-4e04-8982-da36e01f4905"; 
    // A mod-specific numeric value avoids triggering the game's built-in PassedVar handlers.
    private const Interactable.PassedVarType TextVariable = (Interactable.PassedVarType)1_739_501;

    public string TypeId { get; }
    public string TypeName { get; }
    public string MessageId { get; }
    public string SourceTypeId { get; set; } = SourceType_Password; // existing password note as base
    public Action<string> Log { get; set; } = LiveYourLifePlugin.Logger.LogInfo;
    
    public BaseCustomDocument(string typeId)
    {
        TypeId = typeId;
        MessageId = $"LiveYourLife.{TypeId}.Body";
        TypeName = GetType().Name;
    }
    
    private bool RegisterType()
    {
        var trees = Toolbox.Instance.allDDSTrees;
        if (trees.ContainsKey(TypeId))
            return true;

        if (!trees.TryGetValue(SourceTypeId, out var source))
            return false;

        var sourceMessage = FindBodyLayout(source);
        if (sourceMessage == null || source.document == null)
            return false;

        var message = CopyBodyLayout(sourceMessage);
        var tree = new DDSSaveClasses.DDSTreeSave
        {
            id = TypeId,
            name = GetType().Name,
            treeType = source.treeType,
            document = source.document,
            startingMessage = message.instanceID,
        };
        tree.messages.Add(message);
        trees.Add(TypeId, tree);
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

    private DDSSaveClasses.DDSMessageSettings CopyBodyLayout(DDSSaveClasses.DDSMessageSettings source)
    {
        return new DDSSaveClasses.DDSMessageSettings
        {
            msgID = MessageId,
            instanceID = $"LiveYourLife.{TypeId}.BodyLayout",
            elementName = TypeId,
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
    
    protected Interactable? PlaceAt(string text, Human author, IPlacement placement)
    {
        var (position, errorMessage) = placement.CreateWorldPosition();
        if (position == null)
        {
            Log($"{TypeName} skipped: {errorMessage}");
            return null;
        }

        return PlaceAt(text, author, position.Value, placement.CreateWorldRotation());
    }
    
    protected Interactable? PlaceAt(string text, Human author, Vector3 worldPosition, Vector3 worldRotation)
    {
        if (!RegisterType())
        {
            Log($"{TypeName} skipped: DDS document layout unavailable.");
            return null;
        }

        // Passed variables and the DDS override are serialized by the game.
        var variables = new List<Interactable.Passed>();
        variables.Add(new Interactable.Passed(TextVariable, 0f, text));
        var note = InteractableCreator.Instance.CreateWorldInteractable(
            InteriorControls.Instance.note, 
            author, 
            author, 
            author,
            worldPosition,
            worldRotation, 
            variables, 
            null, 
            TypeId);
        note.evidence?.AddOrSetCustomName(Evidence.DataKey.name, TypeName);
        return note;
    }
    
    public string? ReadText(Interactable? note)
    {
        if (note == null || note.pv == null)
        {
            return null;
        }
        
        foreach (var variable in note.pv)
        {
            if (variable.varType == TextVariable)
            {
                return variable.str;
            }
        }
        return null;
    }
}
