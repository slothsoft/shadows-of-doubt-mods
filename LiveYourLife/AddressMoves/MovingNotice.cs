using System.Linq;
using LiveYourLife.Common;
using LiveYourLife.CustomDocuments;

namespace LiveYourLife.AddressMoves;

public class MovingNotice() : BaseCustomDocument("61851517-4896-47a5-ad59-46af1fb9cb17")
{
    public static void Place(NewAddress oldAddress, NewAddress newAddress, Human[] movers)
    {
        var movingNotice = new MovingNotice();
        var createdNote = movingNotice.PlaceAt(
            CreateText(oldAddress, newAddress,  movers),
            movers[0],
            new DoorMatPlacement(oldAddress));
       
        if (createdNote == null)
        {
            movingNotice.Log($"{movingNotice.TypeName} could not be created at {oldAddress.name}.");
            return;
        }
        
        movingNotice.Log($"{movingNotice.TypeName} created at {oldAddress.name}: id={createdNote.id}");
    }
    
    private static string CreateText(NewAddress oldAddress, NewAddress newAddress, Human[] movers)
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
}
