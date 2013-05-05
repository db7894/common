using System;
using System.Collections.Generic;

namespace SharedAssemblies.General.Threading.UnitTests.TestClasses
{
    /// <summary>
    /// Test class for keeping track of what a consumer did
    /// </summary>
    public class ConsumerTestClass
    {
        /// <summary>
        /// True if the on consume started is called
        /// </summary>
        public bool HasOnConsumeStartedBeenCalled { get; set; }


        /// <summary>
        /// True if the on consume stopped was called
        /// </summary>
        public bool HasOnConsumeStoppedBeenCalled { get; set; }


        /// <summary>
        /// True if should fail on this event
        /// </summary>
        public bool ShouldFailOnStart { get; set; }


        /// <summary>
        /// True if should fail on this event
        /// </summary>
        public bool ShouldFailOnStop { get; set; }


        /// <summary>
        /// True if should fail on this event
        /// </summary>
        public bool ShouldFailOnConsume { get; set; }


        /// <summary>
        /// Number of items consumed
        /// </summary>
        public int ItemsConsumedCount { get; set; }


        /// <summary>
        /// The list of all items consumed
        /// </summary>
        public List<string> ItemsConsumed { get; set; }


        /// <summary>
        /// Constructor to create the initial list with size zero
        /// </summary>
        public ConsumerTestClass()
        {
            ItemsConsumed = new List<string>();
        }


        /// <summary>
        /// Callback for when consume started
        /// </summary>
        public void OnConsumeStarted()
        {
            HasOnConsumeStartedBeenCalled = true;

            if(ShouldFailOnStart)
            {
                throw new Exception("You told me to fail");
            }
        }


        /// <summary>
        /// Callback for when consume stopped
        /// </summary>
        public void OnConsumeStopped()
        {
            HasOnConsumeStoppedBeenCalled = true;

            if (ShouldFailOnStop)
            {
                throw new Exception("You told me to fail");
            }
        }


        /// <summary>
        /// Callback for consuming an item
        /// </summary>
        /// <param name="item">The item to consume</param>
        public void OnConsume(string item)
        {
            ++ItemsConsumedCount;
            ItemsConsumed.Add(item);

            if (ShouldFailOnConsume)
            {
                throw new Exception("You told me to fail");
            }
        }


        /// <summary>
        /// Callback for consuming a list of items
        /// </summary>
        /// <param name="items">The array of items to consume</param>
        public void OnConsumeBulk(List<string> items)
        {
            ItemsConsumedCount += items.Count;
            ItemsConsumed.AddRange(items);

            if (ShouldFailOnConsume)
            {
                throw new Exception("You told me to fail");
            }
        }
    }
}
