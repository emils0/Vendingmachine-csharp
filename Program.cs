using System.Diagnostics;

namespace Flaskeautomaten;

internal static class Program {
    public static void Main() {
        Thread producer = new Thread(Producer.ProduceBottles);
        Thread splitter = new Thread(Splitter.StartSplitter);
        Thread consumeBeer = new Thread(Consumer.ConsumeBeer);
        Thread consumeSoda = new Thread(Consumer.ConsumeSoda);

        producer.Start();
        splitter.Start();
        consumeBeer.Start();
        consumeSoda.Start();
    }
}

public static class Producer {
    public static readonly Queue<Bottle> BottleList = new();
    private static int _bottleId;

    public static void ProduceBottles() {
        while (true) {
            Thread.Sleep(1000);
            _bottleId++;

            lock (BottleList) {
                BottleList.Enqueue(new Bottle(_bottleId));
            }

            Debug.WriteLine($"The queue contains {BottleList.Count} bottles.");
        }
    }
}

public static class Splitter {
    public static readonly Queue<Soda> SodaList = new();
    public static readonly Queue<Beer> BeerList = new();
    

    public static void StartSplitter() {
        while (true) {
            if (Producer.BottleList.Count != 0) {
                switch (Random.Shared.Next(0, 2)) {
                    case 0:
                    SodaList.Enqueue(new Soda(Producer.BottleList.Peek().BottleId));
                    Console.WriteLine($"There are now {SodaList.Count} sodas left.");
                    break;
                    
                    case 1:
                    BeerList.Enqueue(new Beer(Producer.BottleList.Peek().BottleId));
                    Console.WriteLine($"There are now {BeerList.Count} beers left.");
                    break;
                }

                lock (Producer.BottleList) {
                    Producer.BottleList.Dequeue();
                }
            }

            Thread.Sleep(1200);
        }
    }
}

public static class Consumer {
    public static void ConsumeBeer() {
        while (true) {
           Thread.Sleep(Random.Shared.Next(500, 5500));

           if (Splitter.BeerList.Count != 0) {
               lock (Splitter.BeerList) {
                   Console.WriteLine($"Beer number {Splitter.BeerList.Peek().BottleId} has been consumed.");
                   Splitter.BeerList.Dequeue();
               }
           }
           else {
               Console.WriteLine("Attempted to consume beer, but there is no beer left. :(");
           }
        }
    }
    
    
    public static void ConsumeSoda() {
        while (true) {
           Thread.Sleep(Random.Shared.Next(500, 5500));

           if (Splitter.SodaList.Count != 0) {
               lock (Splitter.SodaList) {
                   Console.WriteLine($"Soda number {Splitter.SodaList.Peek().BottleId} has been consumed.");
                   Splitter.SodaList.Dequeue();
               }
           }
           else {
               Console.WriteLine("Attempted to consume soda, but there is no soda left. :(");
           }
        }
    }
}

public class Bottle {
    public readonly int BottleId;

    public Bottle(int bottleId) {
        BottleId = bottleId;
    }
}

public class Soda : Bottle {
    public Soda(int bottleId) : base(bottleId) {
    }
}

public class Beer : Bottle {
    public Beer(int bottleId) : base(bottleId) {
    }
}