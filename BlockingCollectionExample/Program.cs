using System.Collections.Concurrent;


class Program
{
    static BlockingCollection<int> bc;

    static void producer()
    {
        for (int i = 0; i < 100; i++)
        {
            bc.Add(i * i);
            Console.WriteLine("Производится число + " + i * i);
        }
        bc.CompleteAdding(); //Помечает экземпляры BlockingCollection<T> как не допускающие добавления дополнительных элементов.
        /*
            Элементы коллекции могут параллельно удаляться несколькими потребителями. Если коллекция становится пустой, то потоки-потребители 
            перейдут в состояние блокировки, пока поток-создатель не добавит хотя бы один элемент. Создаваемый поток может вызвать CompleteAdding метод,
            чтобы указать, что больше элементов не будет добавлено. Потребители могут отслеживать свойство IsCompleted, позволяющее определить, 
            что коллекция опустела, а новые элементы добавляться не будут.
        */
    }

    static void consumer()
    {
        int i;
        while (!bc.IsCompleted) // Возвращает значение, указывающее, помечена ли данная коллекция BlockingCollection<T> как закрытая для добавления элементов и является ли она пустой.
        {
            if (bc.TryTake(out i))
                Console.WriteLine("Потребляется число: - " + i);
        }
    }

    static void Main()
    {
        bc = new BlockingCollection<int>(4);

        // Создадим задачи поставщика и потребителя
        Task Pr = new Task(producer);
        Task Cn = new Task(consumer);

        // Запустим задачи
        Pr.Start();
        Cn.Start();

        try
        {
            Task.WaitAll(Cn, Pr);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            Cn.Dispose();
            Pr.Dispose();
            bc.Dispose();
        }

        Console.ReadLine();
    }
}