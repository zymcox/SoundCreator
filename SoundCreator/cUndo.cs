namespace SoundCreator {

	public struct Items {
		public OscillatorData[] OD;
		public MixerData MD;
	}

	internal class cUndo {
		private static int NoItems = 64;
		private static int x = NoItems - 1;
		private static int StackPointer = 0;
		private static int RedoPointer = 0;

		private static int RingBufferStartPointer = 0;

		private static bool UndoButton = false;
		private static bool RedoButton = false;

		private static int BufferItems = 0;
		private static bool BufferFull = false;

		private static Items[] Item;
		private static Items LastItem;

		public cUndo() {
			Item = new Items[NoItems];
			LastItem = new Items();
		}

		public void InsertNew( Items NewItem ) {
			if(LastItem.Equals(NewItem)) {
				return;    //// Funkar INTE !!!!!
			}
			LastItem = NewItem;
			BufferItems++;
			if (BufferItems == NoItems) BufferFull = true;
			if (BufferItems == 1 && !BufferFull) UndoButton = false; else UndoButton = true;
			RedoButton = false;
			if (StackPointer == ((RingBufferStartPointer + NoItems) & x) && BufferFull) {
				RingBufferStartPointer = (RingBufferStartPointer + 1) & x;
			}
			Push(NewItem);
			StackPointer = (StackPointer + 1) & x;
			RedoPointer = StackPointer;
		}

		public Items Undo() {
			BufferItems--;
			RedoButton = true;
			StackPointer = (StackPointer - 2) & x;
			if (StackPointer == RingBufferStartPointer || (BufferItems == 0 && !BufferFull)) UndoButton = false;
			if (StackPointer == ((RingBufferStartPointer - 1) & x)) {
				StackPointer = RingBufferStartPointer;
				UndoButton = false;
			}
			Items a = Pull();
			StackPointer = (StackPointer + 1) & x;
			return a;
		}

		public Items Redo() {
			BufferItems++;
			UndoButton = true;
			if (StackPointer == RingBufferStartPointer) UndoButton = false;
			Items a = Pull();
			StackPointer = (StackPointer + 1) & x;
			if (RedoPointer == StackPointer) RedoButton = false;
			return a;
		}

		private void Push( Items a ) {
			Item[StackPointer & x] = a;
		}

		private Items Pull() {
			return Item[StackPointer & x];
		}

		public bool GetUndoButton() {
			return UndoButton;
		}

		public bool GetRedoButton() {
			return RedoButton;
		}
	}
}