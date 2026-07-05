

using Manager;

namespace Core
{
    public class GameManager: ManagerBase<GameManager>
    {
        private string _folder;
        private string _tableName;
        
        public string Folder
        {
            get => _folder;
            set => _folder = value;
        }

        public string TableName
        {
            get => _tableName;
            set => _tableName = value;
        }        
    }
}