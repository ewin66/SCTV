using System;
using System.Collections.Generic;
using System.Text;

namespace SCTVJustinTVFileManager
{
    class JustinRecord
    {
        string Image_url_medium;
        string Video_rotation;
        string Created_on;
        string Stream_name;
        string Filtered;
        string Video_bitrate;
        string Updated_on;
        string Id;
        string File_name;
        string Broadcaster;
        string Save_forever;
        string Broadcast_id;
        string User_id;
        string Origin_name;
        string File_size;
        string Deleted_by_user;
        string Broadcast_part;
        string Video_file_url;
        string Length;
        string Servers;
        string Start_time;
        string Last_part;

        public string image_url_medium
        {
            get{ return Image_url_medium; }
            set { Image_url_medium = value; }
        }

        public string video_rotation
        {
            get{ return Video_rotation; }
            set { Video_rotation = value; }
        }

        public string created_on
        {
            get{ return Created_on; }
            set { Created_on = value; }
        }

        public string stream_name
        {
            get{ return Stream_name; }
            set { Stream_name = value; }
        }
        
        public string filtered
        {
            get{ return Filtered; }
            set { Filtered = value; }
        }

        public string video_bitrate
        {
            get{ return Video_bitrate; }
            set { Video_bitrate = value; }
        }

        public string updated_on
        {
            get{ return Updated_on; }
            set { Updated_on = value; }
        }

        public string save_forever
        {
            get{ return Save_forever; }
            set { Save_forever = value; }
        }

        public string id
        {
            get{ return Id; }
            set { Id = value; }
        }

        public string file_name
        {
            get{ return File_name; }
            set { File_name = value; }
        }

        public string broadcaster
        {
            get{ return Broadcaster; }
            set { Broadcaster = value; }
        }

        public string broadcast_id
        {
            get{ return Broadcast_id; }
            set { Broadcast_id = value; }
        }

        public string user_id
        {
            get{ return User_id; }
            set { User_id = value; }
        }

        public string origin_name
        {
            get{ return Origin_name; }
            set { Origin_name = value; }
        }

        public string file_size
        {
            get{ return File_size; }
            set { File_size = value; }
        }

        public string deleted_by_user
        {
            get{ return Deleted_by_user; }
            set { Deleted_by_user = value; }
        }

        public string broadcast_part
        {
            get{ return Broadcast_part; }
            set { Broadcast_part = value; }
        }

        public string video_file_url
        {
            get{ return Video_file_url; }
            set { Video_file_url = value; }
        }

        public string length
        {
            get{ return Length; }
            set { Length = value; }
        }

        public string servers
        {
            get{ return Servers; }
            set { Servers = value; }
        }

        public string start_time
        {
            get{ return Start_time; }
            set { Start_time = value; }
        }

        public string last_part
        {
            get{ return Last_part; }
            set { Last_part = value; }
        }
    }
}
