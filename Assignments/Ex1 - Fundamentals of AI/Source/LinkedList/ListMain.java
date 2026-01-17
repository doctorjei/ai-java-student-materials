import ai_structures.LinkedList;
import java.util.function.Consumer;

public class ListMain
{
    public static <T> void traverse(LinkedList<T> q, Consumer<T> dataFunction)
    {
        System.out.print("[ ");

        for (T item : q)
        {
            dataFunction.accept(item);
            System.out.print(" ");
        }

        System.out.println("]");
    }

    public static void main(String[] args)
    {
        String[] paddingArray = { "       ", "      ", "     ", "    ", "   ", "  ", " ", "" };

        // Prepare lists.
        var referenceList = new LinkedList<String>(); // Strings
        var valueList = new LinkedList<Character>(); // Characters

        String[] testStrings = { "alpha", "bravo", "charlie",
                               "charlie", "dog", "echo", "foxtrot", "golf" };

        // Lambdas for testing / printing.
        Consumer<String> printString = testString -> System.out.print(testString);
        Consumer<Character> printChar = testChar -> System.out.print(testChar);

        // Test isEmpty function.
        assert referenceList.isEmpty() && valueList.isEmpty();

        // Test enqueue.
        System.out.println("=============================");
        System.out.println("| METHOD TESTS - LinkedList |");
        System.out.println("=============================");
        System.out.println();

        System.out.println("enqueue()\n---------");
        for (String item : testStrings)
        {
            referenceList.enqueue(item);
            System.out.print("referenceList: ");
            traverse(referenceList, printString);
        }
        System.out.println();

        for (String item : testStrings)
        {
            valueList.enqueue(item.charAt(0));
            System.out.print("valueList: ");
            traverse(valueList, printChar);
        }
        System.out.println("\n");

        // Test dequeue.
        System.out.println("dequeue(), getFront()\n---------------------");
        for (;;)
        {
            String entryName = referenceList.getFront();
            String padding = paddingArray[entryName.length()];
            System.out.print("Removing front element \"" + entryName + "\" " + padding);
            referenceList.dequeue();
            traverse(referenceList, printString);

            if (referenceList.isEmpty())
                break;
            else
                valueList.dequeue();
        }
        System.out.println();

        System.out.println(".-----------------.\n| Post-Test State |\n'-----------------'");
        System.out.print("referenceList: ");
        traverse(referenceList, printString);
        System.out.print("valueList:     ");
        traverse(valueList, printChar);
        System.out.println();

        // Test removal of only element.
        System.out.print("remove() - removing only element '" + valueList.getFront() + "': ");
        valueList.remove(testStrings[testStrings.length - 1].charAt(0));
        traverse(valueList, printChar);

        // Test containment check when empty
        char tempChar = 'a';
        System.out.print("contain() - searching for uncontained '" + tempChar + "': ");
        if (!valueList.contains(tempChar))
            traverse(valueList, printChar);
        else
            System.out.println("ERROR: found element even though it's not there...");
        System.out.println("\n");

        // Test pop.
        System.out.println("pop(), getBack()\n----------------");

        System.out.print("******** Rebuilt List ********* ");
        for (String item : testStrings)
            referenceList.enqueue(item);
        traverse(referenceList, printString);

        while (!referenceList.isEmpty())
        {
            String entryName = referenceList.getBack();
            String padding = paddingArray[entryName.length()];
            System.out.print("Removing back element \"" + entryName + "\" " + padding);
            referenceList.pop();
            traverse(referenceList, printString);
        }

        // Enqueue after removal and search & remove
        System.out.println("******** Rebuilding Lists (from index of 1) *********");
        for (int index = 1; index < testStrings.length; index++)
        {
            referenceList.enqueue(testStrings[index]);
            valueList.enqueue(testStrings[index].charAt(0));
        }

        System.out.print("referenceList: ");
        traverse(referenceList, printString);
        System.out.print("valueList:     ");
        traverse(valueList, printChar);
        System.out.println("\n");

        // Test removal from various locations; the first should fail.
        System.out.println("contains(), remove()\n--------------------");
        String[] toRemove = { testStrings[0], testStrings[1], testStrings[4], testStrings[7] };

        for (String testMe : toRemove)
        {
            if (testMe == testStrings[0])
                System.out.print("Unsuccessful search & remove: ");
            else
                System.out.print("Search & removal of \"" + testMe + "\": ");

            if (referenceList.contains(testMe))
            {
                System.out.print(paddingArray[testMe.length() + 1]);

                if (!valueList.contains(testMe.charAt(0)))
                    System.out.println("**ERROR: element in referenceList but not valueList**");

                referenceList.remove(testMe);
                valueList.remove(testMe.charAt(0));
                traverse(referenceList, printString);
            }
            else
            {
                System.out.println("*** ELEMENT NOT FOUND. ***");

                if (valueList.contains(testMe.charAt(0)))
                    System.out.println("**ERROR: element in valueList but not referenceList**");
            }
        }
        System.out.println("\n");

        // Test removal of all elements by method.
        System.out.println("clear()\n-------");
        System.out.print("Removing all elements from referenceList: ");
        referenceList.clear();
        traverse(referenceList, printString);

        System.out.println("\nPress ENTER to continue...");
        java.util.Scanner scanner = new java.util.Scanner(System.in);
        scanner.nextLine();
    }
}
