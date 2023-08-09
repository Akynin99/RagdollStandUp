    public interface IBoneClickService : IService
    {
        public void Subscribe(IBoneClickObserver obs);
        public void Unsubscribe(IBoneClickObserver obs);

        
    }
