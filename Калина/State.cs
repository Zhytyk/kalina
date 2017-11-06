using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Globalization;
namespace Калина
{
    public class State
    {
        public byte[,] data;
        public int cols;
        
        public Dictionary<string, State> keys_exp_values, enc_values, dec_values;

        public State()
        {

        }

        public State(Int32 data_length)
        {
            switch (data_length)
            {
                case 128: cols = 2;
                    break;
                case 256: cols = 4;
                    break;
                case 512: cols = 8;
                    break;
            }

            data = new byte[8, cols];
        }

        private void simple_512_shift(int row, int num)
        {
            for (int iter = 0; iter < num; iter++)
            {
                byte buffer = data[row, 7];
                data[row, 7] = data[row, 6];
                data[row, 6] = data[row, 5];
                data[row, 5] = data[row, 4];
                data[row, 4] = data[row, 3];
                data[row, 3] = data[row, 2];
                data[row, 2] = data[row, 1];
                data[row, 1] = data[row, 0];
                data[row, 0] = buffer;
            }
        }

        private void simple_256_shift(int row, int num)
        {
            for (int j = row; j < row + 2; j++)
            {
                for (int i = 0; i < num; i++)
                {
                    byte buffer = data[j, 3];
                    data[j, 3] = data[j, 2];
                    data[j, 2] = data[j, 1];
                    data[j, 1] = data[j, 0];
                    data[j, 0] = buffer;
                }
            }
        }
        public State invert()
        {
            State result = new State(this.cols * 64);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    result.data[i, j] = (byte)~this.data[i, j];
                }
            }
            return result;
        }

        public void XORRoundKey(State round_key)
        {
            for (int j = 0; j < this.cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    this.data[i, j] = Convert.ToByte(this.data[i, j] ^ round_key.data[i, j]);
                }
            }
        }

        public void S_boxes()
        {
            string[] sbox_value = new string[8];

            sbox_value[0] = "a86d3e92dc2e34229beb78b32ff7ac8143f3dd72f2e748334c8411beefea52545f1da39e83e2fc24fe6901cee80a64c006cb4f61375ab728603ae5bd8c0d10ed6bc9b4d142966a365c9d00ae357ed04e754db663e41688c7dad768e903f8d9446c2c9afa7a23a5b218d3988ad45013a759af0eee322b533b4670a0317f1a0c2a71791ff49cc2868ecd67c51cfbc41285dfe0bf19cc65f9777d4002ec05072925879715d5ab665bba21b5a6f1c15751e695fde1ad4a0fdbf5b0de74995eb8b9ca176f49588fbc38143f5d2d94903ccf7cf04bd2a46ea97b9f1b300baa2062d68bd84593bb0447c3088991a2f63de373560939c6a127411e55ffb1762682c88d80";
            sbox_value[1] = "ce42317c1a4ce7302a7d188bf15072b6bb15f6266b632bdcda9332466e7a2cc2eb56642e698e48b7c91b7180942fdd0192b458f7f35bfd6c00fe4b1e2874d0f0ea659e08bdcc964a7ec4ef389a53875acb1cf45d333c45b5a2473be184b3beed13882244ab19fc3f550970b8e8615ea7c143aa3efaa14197bf86a0a8a3afa666e9c5759fd18112d4110be4e04f39ec213a5c0f149b490d62d58f400c7735047fd63602c8687b792d9c9dff23d3dec68ab2bab1ae4ed9e506cf6ac37685cd0327d2f5df54166f89a40e07a91de21f34c790576d1095378ca50ab9e6255299fbc0176773d89160e3833db07824f2acdb29f88d4dbceeca205f5198f90582ad59d7";
            sbox_value[2] = "934a49a62510561e8727b373526bf642d9173c2ca5d5810bc9cf69f9b6f5ff049a2bc0c4d74f8f05f09957ca087913a0b5c2d8e3039e77d65da81f3af3bf58db98945c76114dcc146d50071aae01f13922f49b7800a99c6e3f0f1cfbbe5f478645bbadb7c355b96c88378a0d19750a54fca385b42ec6e27e8d24bcc189637faaba62530992d0ac66c72820fe321bc58c6ae4a13bef7bb8fdf730ebfa2623a734df717a0e4e182fb11d95cef2b03de72102d4c841129715e5e9d28e6fea68618b9fcd2d4c9dd3a460ec3eabbd4b2a5af8dc70e0de7d367cafed5bee966465060c5116d1b2cbe6da5e804031dd84e8467459e17290354838332983a24382914467";
            sbox_value[3] = "68225870b53172ea34c3976ca7e7cb648d032f5d381444bc7e9f2e90c6b0bb6dca460df36eae156210b6f80093826bdc4d3d02450eeefd0cf1d7658e0ff776f0732ded40e5c837247b29f66f0afeba594b4a51ccf448bea68fc27550069d5aa94e539ee8f9d35fa863eb0701e6877d4c2a8311948630aaeca0c004c52b5c7817d413f256e9a19b6705a449da96810b7f528a3e084f9288209a8b3347a335959126b755ced641d8db438ce43f1cdee3b8b3d55e1a85b1ab7c771dd9cdafb4adc95425d13a2318892821fbb9696aa574571e7916d2cfc49cddbfffd0a212fc981b19f53ce1322cfaac27c142e284803be01fbd66df9971605b09b2c77a39ef3661";
            sbox_value[4] = "a86d3e92dc2e34229beb78b32ff7ac8143f3dd72f2e748334c8411beefea52545f1da39e83e2fc24fe6901cee80a64c006cb4f61375ab728603ae5bd8c0d10ed6bc9b4d142966a365c9d00ae357ed04e754db663e41688c7dad768e903f8d9446c2c9afa7a23a5b218d3988ad45013a759af0eee322b533b4670a0317f1a0c2a71791ff49cc2868ecd67c51cfbc41285dfe0bf19cc65f9777d4002ec05072925879715d5ab665bba21b5a6f1c15751e695fde1ad4a0fdbf5b0de74995eb8b9ca176f49588fbc38143f5d2d94903ccf7cf04bd2a46ea97b9f1b300baa2062d68bd84593bb0447c3088991a2f63de373560939c6a127411e55ffb1762682c88d80";
            sbox_value[5] = "ce42317c1a4ce7302a7d188bf15072b6bb15f6266b632bdcda9332466e7a2cc2eb56642e698e48b7c91b7180942fdd0192b458f7f35bfd6c00fe4b1e2874d0f0ea659e08bdcc964a7ec4ef389a53875acb1cf45d333c45b5a2473be184b3beed13882244ab19fc3f550970b8e8615ea7c143aa3efaa14197bf86a0a8a3afa666e9c5759fd18112d4110be4e04f39ec213a5c0f149b490d62d58f400c7735047fd63602c8687b792d9c9dff23d3dec68ab2bab1ae4ed9e506cf6ac37685cd0327d2f5df54166f89a40e07a91de21f34c790576d1095378ca50ab9e6255299fbc0176773d89160e3833db07824f2acdb29f88d4dbceeca205f5198f90582ad59d7";
            sbox_value[6] = "934a49a62510561e8727b373526bf642d9173c2ca5d5810bc9cf69f9b6f5ff049a2bc0c4d74f8f05f09957ca087913a0b5c2d8e3039e77d65da81f3af3bf58db98945c76114dcc146d50071aae01f13922f49b7800a99c6e3f0f1cfbbe5f478645bbadb7c355b96c88378a0d19750a54fca385b42ec6e27e8d24bcc189637faaba62530992d0ac66c72820fe321bc58c6ae4a13bef7bb8fdf730ebfa2623a734df717a0e4e182fb11d95cef2b03de72102d4c841129715e5e9d28e6fea68618b9fcd2d4c9dd3a460ec3eabbd4b2a5af8dc70e0de7d367cafed5bee966465060c5116d1b2cbe6da5e804031dd84e8467459e17290354838332983a24382914467";
            sbox_value[7] = "68225870b53172ea34c3976ca7e7cb648d032f5d381444bc7e9f2e90c6b0bb6dca460df36eae156210b6f80093826bdc4d3d02450eeefd0cf1d7658e0ff776f0732ded40e5c837247b29f66f0afeba594b4a51ccf448bea68fc27550069d5aa94e539ee8f9d35fa863eb0701e6877d4c2a8311948630aaeca0c004c52b5c7817d413f256e9a19b6705a449da96810b7f528a3e084f9288209a8b3347a335959126b755ced641d8db438ce43f1cdee3b8b3d55e1a85b1ab7c771dd9cdafb4adc95425d13a2318892821fbb9696aa574571e7916d2cfc49cddbfffd0a212fc981b19f53ce1322cfaac27c142e284803be01fbd66df9971605b09b2c77a39ef3661";

            for (int i = 0; i < 8; i++)
            {
                int start = 0;
                byte[,] sbox = new byte[16, 16];

                for (int n = 0; n < 16; n++)
                {
                    for (int m = 0; m < 16; m++)
                    {
                        if (start <= sbox_value[i].Length - 2)
                        {
                            sbox[m, n] = Convert.ToByte(sbox_value[i].Substring(start, 2), 16);
                            start = start + 2;
                        }
                    }
                }


                for (int j = 0; j < cols; j++)
                {
                    int x = this.data[i, j] >> 4;
                    int y = this.data[i, j] & 15;
                    this.data[i, j] = sbox[x, y];
                }

            }
        }

        public void Shift_Rows()
        {
            byte buffer;
            #region shift128
            if (cols == 2)
            {
                for (int i = 4; i < 8; i++)
                {
                    buffer = data[i, 1];
                    data[i, 1] = data[i, 0];
                    data[i, 0] = buffer;
                }
            }
            #endregion
            #region shift256
            if (cols == 4)
            {
                simple_256_shift(2, 1);
                simple_256_shift(4, 2);
                simple_256_shift(6, 3);
            }
            #endregion
            #region shift512
            if (cols == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    simple_512_shift(i, i);
                }
            }
            #endregion
        }

        public void Add32RoundKey(State key)
        {
            Int32 data_word, key_word, temp;

            for (int j = 0; j < this.cols; j++)
            {
                key_word = 0; data_word = 0;
                #region first word
                for (int i = 3; i > -1; i--)
                {
                    temp = Convert.ToInt32(this.data[i, j]);
                    temp <<= 8 * i;
                    data_word = data_word ^ temp;

                    temp = Convert.ToInt32(key.data[i, j]);
                    temp <<= 8 * i;
                    key_word = key_word ^ temp;
                }
                data_word += key_word;
                string data = Convert.ToString(data_word, 16);
                while (data.Length < 8)
                {
                    data = "0" + data;
                }

                int start = 0;
                for (int i = 3; i > -1; i--)
                {
                    this.data[i, j] = Convert.ToByte(data.Substring(start, 2), 16);
                    start = start + 2;
                }
                #endregion
                #region second_word

                data_word = 0; key_word = 0;

                for (int i = 7; i > 3; i--)
                {
                    temp = Convert.ToInt32(this.data[i, j]);
                    temp <<= 8 * (i % 4);
                    data_word = data_word ^ temp;

                    temp = Convert.ToInt32(key.data[i, j]);
                    temp <<= 8 * (i % 4);
                    key_word = key_word ^ temp;
                }
                data_word += key_word;

                data = Convert.ToString(data_word, 16);
                while (data.Length < 8)
                {
                    data = "0" + data;
                }

                start = 0;

                for (int i = 7; i > 3; i--)
                {
                    this.data[i, j] = Convert.ToByte(data.Substring(start, 2), 16);
                    start = start + 2;
                }
                #endregion

            }
        }

        public void Mix_Columns()
        {
            for (int j = 0; j < cols; j++)
            {
                #region temp

                byte a = this.data[0, j];
                byte b = this.data[1, j];
                byte c = this.data[2, j];
                byte d = this.data[3, j];
                byte e = this.data[4, j];
                byte f = this.data[5, j];
                byte g = this.data[6, j];
                byte h = this.data[7, j];

                #endregion

                this.data[0, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x05, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x08, e) ^ Form1.GMul(0x06, f) ^ Form1.GMul(0x07, g) ^ Form1.GMul(0x04, h));
                this.data[1, j] = (byte)(Form1.GMul(0x04, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x05, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x08, f) ^ Form1.GMul(0x06, g) ^ Form1.GMul(0x07, h));
                this.data[2, j] = (byte)(Form1.GMul(0x07, a) ^ Form1.GMul(0x04, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x05, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x08, g) ^ Form1.GMul(0x06, h));
                this.data[3, j] = (byte)(Form1.GMul(0x06, a) ^ Form1.GMul(0x07, b) ^ Form1.GMul(0x04, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x05, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x08, h));
                this.data[4, j] = (byte)(Form1.GMul(0x08, a) ^ Form1.GMul(0x06, b) ^ Form1.GMul(0x07, c) ^ Form1.GMul(0x04, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x05, g) ^ Form1.GMul(0x01, h));
                this.data[5, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x08, b) ^ Form1.GMul(0x06, c) ^ Form1.GMul(0x07, d) ^ Form1.GMul(0x04, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x05, h));
                this.data[6, j] = (byte)(Form1.GMul(0x05, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x08, c) ^ Form1.GMul(0x06, d) ^ Form1.GMul(0x07, e) ^ Form1.GMul(0x04, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x01, h));
                this.data[7, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x05, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x08, d) ^ Form1.GMul(0x06, e) ^ Form1.GMul(0x07, f) ^ Form1.GMul(0x04, g) ^ Form1.GMul(0x01, h));
            }
        }


        public void Sub32RoundKey(State key)
        {
            Int32 data_word, key_word, temp;

            for (int j = 0; j < this.cols; j++)
            {
                key_word = 0; data_word = 0;
                #region first word
                for (int i = 3; i > -1; i--)
                {
                    temp = Convert.ToInt32(this.data[i, j]);
                    temp <<= 8 * i;
                    data_word = data_word ^ temp;

                    temp = Convert.ToInt32(key.data[i, j]);
                    temp <<= 8 * i;
                    key_word = key_word ^ temp;
                }
                data_word = data_word - key_word;

                string data = Convert.ToString(data_word, 16);
                while (data.Length < 8)
                {
                    data = "0" + data;
                }

                int start = 0;

                for (int i = 3; i > -1; i--)
                {
                    this.data[i, j] = Convert.ToByte(data.Substring(start, 2), 16);
                    start = start + 2;
                }
                #endregion
                #region second_word

                data_word = 0; key_word = 0;

                for (int i = 7; i > 3; i--)
                {
                    temp = Convert.ToInt32(this.data[i, j]);
                    temp <<= 8 * (i % 4);
                    data_word = data_word ^ temp;

                    temp = Convert.ToInt32(key.data[i, j]);
                    temp <<= 8 * (i % 4);
                    key_word = key_word ^ temp;
                }
                data_word = data_word - key_word;

                data = Convert.ToString(data_word, 16);
                while (data.Length < 8)
                {
                    data = "0" + data;
                }

                start = 0;
                for (int i = 7; i > 3; i--)
                {
                    this.data[i, j] = Convert.ToByte(data.Substring(start, 2), 16);
                    start = start + 2;
                }
                #endregion
            }
        }
        public void Inv_S_boxes()
        {
            string[] inv_sbox = new string[8];

            inv_sbox[0] = "a4e3cd9d99d68397ffcca78bf2e4190da2a18ab75fea3308f09e3f9fca342bbaa9e8704744e1dd31cf30ae76582d5241c5e65671016735ee422e22b06e96de114e7c7260f5f1e2ab91bc3d24d8c64538c92af9c41e7f5905f80b669aa83aa37b0355bf7487fe5aaf6854aa252fedfabed90c4f435eda98790a1af663759551d07e86736c613ca5a065a600dbdf0ec2d50f39e91f2c0792188ebb5deb14e5b569d2d7f7934b536446b626bd7afb85d136ad8d57771d6a046dfd804a3e136b90c8e7b816dc818406fcc348e05c4940b962d312acce159c1089ef943bb38821f31b276f5020f4cb4dd47832b4b1b29b37825b28c08c23021cc74c7d1729ec09c18f";
            inv_sbox[1] = "833d6f70a9d05ea6b20da79efde3b8f3f288f820678fd6a2584e572b0748b5c02a6826a110cd79e0cf03850bf10cccceeb06ba4517d4512e7e91c7d5abca6e43e939beec363c22d3c5c27c139478a825bf11bdd965861428cb4d7d7518896b1c7b4c311ab112f7bb9764e7f0ea0aad219c0efb5d951d1ec9e477f672fcff603334a0c3b462234aae169fb7b63a3ec60f9656fed859ef426a6cddac9d825b08af8d40800974f49bd1fac4271b5f810447989261a5a353415ab049460105ee02edb915e155501973306d8ade3f5471e86669bc7a8e2f352d901f9adf44dbe2f5638cb332374be6c18452243be500da4f9329dcd276c87f5cf99938d7878b2ca4aa";
            inv_sbox[2] = "4550a8999e947c1d8e3ff2ca22582d82d444faae3b0eeb1a61df297ab72e1fe40b4b05c8f0c0182fcf48af3e139b67ba43e2d97fbf28d7b09f0017a0465c33c3f17497f9eff6cdfece146c37321b1915726b404f0656ddd6279a4103e8517bd1ed1ec95dee607834f5bd30c157735ee0a4119096e5a2ff63805be9368842ea89c25a986f5fe3db35860493692b23defc38c68ff4200fa1d2c792556681018bb1e6b4dcb310ec092aa602f708b26ecbb971d81239cc9dd059fb25ac164ef3a9b5fda531213c24766df86568a7640d8c07b68a2cda5483844d874c26bc1cbe8d793a70479c4a7e7577ab53c4c5aa3dadb895a36a8552d5bbe7620c7dd3910a49e1";
            inv_sbox[3] = "b282795734b56f30deb187d197adfef3b6278c505a25ff5fd8f9585b9e2c3e8323cd104eae097260d259bd9f953dbe2811184ca98a16860417c2c90b9056ea32a7517480610cf0ecce3798db5d08aa4588621cd9332aa3a54be9dc40b71b441ec52d0aef12382fe347c87592c14ac6a4a6f78e64b9fc788bd6a0c01aaf93d0d3395c7c41552000e769ed76fa546a36a28f0e94cfa8f4bc1d6c4ff5acfbab4846c43b073c15e5ccbf198967e402b8706ee8fdc7ee057fe28499686be1e07a969c73ca5e2ef6d7b07b9a6d7e7135f277dd229b14130331f1e601d5eb1fbb7d2463430da129062b4281b32652653ada53d4c30f21ba4966b4f88591cb8d4d3fdf9d";
            inv_sbox[4] = "a4e3cd9d99d68397ffcca78bf2e4190da2a18ab75fea3308f09e3f9fca342bbaa9e8704744e1dd31cf30ae76582d5241c5e65671016735ee422e22b06e96de114e7c7260f5f1e2ab91bc3d24d8c64538c92af9c41e7f5905f80b669aa83aa37b0355bf7487fe5aaf6854aa252fedfabed90c4f435eda98790a1af663759551d07e86736c613ca5a065a600dbdf0ec2d50f39e91f2c0792188ebb5deb14e5b569d2d7f7934b536446b626bd7afb85d136ad8d57771d6a046dfd804a3e136b90c8e7b816dc818406fcc348e05c4940b962d312acce159c1089ef943bb38821f31b276f5020f4cb4dd47832b4b1b29b37825b28c08c23021cc74c7d1729ec09c18f";
            inv_sbox[5] = "833d6f70a9d05ea6b20da79efde3b8f3f288f820678fd6a2584e572b0748b5c02a6826a110cd79e0cf03850bf10cccceeb06ba4517d4512e7e91c7d5abca6e43e939beec363c22d3c5c27c139478a825bf11bdd965861428cb4d7d7518896b1c7b4c311ab112f7bb9764e7f0ea0aad219c0efb5d951d1ec9e477f672fcff603334a0c3b462234aae169fb7b63a3ec60f9656fed859ef426a6cddac9d825b08af8d40800974f49bd1fac4271b5f810447989261a5a353415ab049460105ee02edb915e155501973306d8ade3f5471e86669bc7a8e2f352d901f9adf44dbe2f5638cb332374be6c18452243be500da4f9329dcd276c87f5cf99938d7878b2ca4aa";
            inv_sbox[6] = "4550a8999e947c1d8e3ff2ca22582d82d444faae3b0eeb1a61df297ab72e1fe40b4b05c8f0c0182fcf48af3e139b67ba43e2d97fbf28d7b09f0017a0465c33c3f17497f9eff6cdfece146c37321b1915726b404f0656ddd6279a4103e8517bd1ed1ec95dee607834f5bd30c157735ee0a4119096e5a2ff63805be9368842ea89c25a986f5fe3db35860493692b23defc38c68ff4200fa1d2c792556681018bb1e6b4dcb310ec092aa602f708b26ecbb971d81239cc9dd059fb25ac164ef3a9b5fda531213c24766df86568a7640d8c07b68a2cda5483844d874c26bc1cbe8d793a70479c4a7e7577ab53c4c5aa3dadb895a36a8552d5bbe7620c7dd3910a49e1";
            inv_sbox[7] = "b282795734b56f30deb187d197adfef3b6278c505a25ff5fd8f9585b9e2c3e8323cd104eae097260d259bd9f953dbe2811184ca98a16860417c2c90b9056ea32a7517480610cf0ecce3798db5d08aa4588621cd9332aa3a54be9dc40b71b441ec52d0aef12382fe347c87592c14ac6a4a6f78e64b9fc788bd6a0c01aaf93d0d3395c7c41552000e769ed76fa546a36a28f0e94cfa8f4bc1d6c4ff5acfbab4846c43b073c15e5ccbf198967e402b8706ee8fdc7ee057fe28499686be1e07a969c73ca5e2ef6d7b07b9a6d7e7135f277dd229b14130331f1e601d5eb1fbb7d2463430da129062b4281b32652653ada53d4c30f21ba4966b4f88591cb8d4d3fdf9d";

            for (int i = 0; i < 8; i++)
            {
                int start = 0;
                byte[,] invsbox = new byte[16, 16];

                #region invsbox_forming
                for (int n = 0; n < 16; n++)
                {
                    for (int m = 0; m < 16; m++)
                    {
                        if (start <= inv_sbox[i].Length - 2)
                        {
                            invsbox[m, n] = Convert.ToByte(inv_sbox[i].Substring(start, 2), 16);
                            start = start + 2;
                        }
                    }
                }

                #endregion
                #region substitution
                for (int j = 0; j < cols; j++)
                {
                    int x = this.data[i, j] >> 4;
                    int y = this.data[i, j] & 15;
                    this.data[i, j] = invsbox[x, y];
                }
                #endregion
            }

        }
        public void Inv_Shift_Rows()
        {
            byte buffer;
            #region shift128
            if (this.cols == 2)
            {
                for (int i = 4; i < 8; i++)
                {
                    buffer = data[i, 1];
                    data[i, 1] = data[i, 0];
                    data[i, 0] = buffer;
                }
            }
            #endregion

            #region shift256
            if (this.cols == 4)
            {
                simple_256_shift(2, 3);
                simple_256_shift(4, 2);
                simple_256_shift(6, 1);
            }
            #endregion

            #region shift512
            if (this.cols == 8)
            {
                for (int i = 1; i < 8; i++)
                {
                    simple_512_shift(i, 8 - i);
                }
            }
            #endregion
        }
        public void Inv_MixColumns()
        {
            for (int j = 0; j < cols; j++)
            {
                byte a = data[0, j];
                byte b = data[1, j];
                byte c = data[2, j];
                byte d = data[3, j];
                byte e = data[4, j];
                byte f = data[5, j];
                byte g = data[6, j];
                byte h = data[7, j];

                this.data[0, j] = (byte)(Form1.GMul(0xad, a) ^ Form1.GMul(0x95, b) ^ Form1.GMul(0x76, c) ^ Form1.GMul(0xa8, d) ^ Form1.GMul(0x2f, e) ^ Form1.GMul(0x49, f) ^ Form1.GMul(0xd7, g) ^ Form1.GMul(0xca, h));
                this.data[1, j] = (byte)(Form1.GMul(0xca, a) ^ Form1.GMul(0xad, b) ^ Form1.GMul(0x95, c) ^ Form1.GMul(0x76, d) ^ Form1.GMul(0xa8, e) ^ Form1.GMul(0x2f, f) ^ Form1.GMul(0x49, g) ^ Form1.GMul(0xd7, h));
                this.data[2, j] = (byte)(Form1.GMul(0xd7, a) ^ Form1.GMul(0xca, b) ^ Form1.GMul(0xad, c) ^ Form1.GMul(0x95, d) ^ Form1.GMul(0x76, e) ^ Form1.GMul(0xa8, f) ^ Form1.GMul(0x2f, g) ^ Form1.GMul(0x49, h));
                this.data[3, j] = (byte)(Form1.GMul(0x49, a) ^ Form1.GMul(0xd7, b) ^ Form1.GMul(0xca, c) ^ Form1.GMul(0xad, d) ^ Form1.GMul(0x95, e) ^ Form1.GMul(0x76, f) ^ Form1.GMul(0xa8, g) ^ Form1.GMul(0x2f, h));
                this.data[4, j] = (byte)(Form1.GMul(0x2f, a) ^ Form1.GMul(0x49, b) ^ Form1.GMul(0xd7, c) ^ Form1.GMul(0xca, d) ^ Form1.GMul(0xad, e) ^ Form1.GMul(0x95, f) ^ Form1.GMul(0x76, g) ^ Form1.GMul(0xa8, h));
                this.data[5, j] = (byte)(Form1.GMul(0xa8, a) ^ Form1.GMul(0x2f, b) ^ Form1.GMul(0x49, c) ^ Form1.GMul(0xd7, d) ^ Form1.GMul(0xca, e) ^ Form1.GMul(0xad, f) ^ Form1.GMul(0x95, g) ^ Form1.GMul(0x76, h));
                this.data[6, j] = (byte)(Form1.GMul(0x76, a) ^ Form1.GMul(0xa8, b) ^ Form1.GMul(0x2f, c) ^ Form1.GMul(0x49, d) ^ Form1.GMul(0xd7, e) ^ Form1.GMul(0xca, f) ^ Form1.GMul(0xad, g) ^ Form1.GMul(0x95, h));
                this.data[7, j] = (byte)(Form1.GMul(0x95, a) ^ Form1.GMul(0x76, b) ^ Form1.GMul(0xa8, c) ^ Form1.GMul(0x2f, d) ^ Form1.GMul(0x49, e) ^ Form1.GMul(0xd7, f) ^ Form1.GMul(0xca, g) ^ Form1.GMul(0xad, h));
            }
        }


        public void add_hex(string datastr)
        {
            int start = 0;

            for (int j = 0; j < this.cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (start <= datastr.Length - 2)
                    {
                        this.data[i, j] = Convert.ToByte(datastr.Substring(start, 2), 16);
                        start = start + 2;
                    }
                }
            }
        }
        public void add_text(byte[] massive)
        {
            int index = 0;

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    this.data[i, j] = massive[index];
                    index++;
                }
            }
        }
        public State add_part(int data_length, int start_column)
        {
            State result = new State(data_length);

            for (int cols = 0; cols < data_length / 64; cols++)
            {
                for (int rows = 0; rows < 8; rows++)
                {
                    result.data[rows, cols] = this.data[rows, (cols + start_column)];
                }
            }

            return result;
        }
        //public void show(TextBox l)
        //{
        // string tmp = "";

        // for (int j = 0; j < 8; j++)
        //{
        // for (int i = 0; i < cols; i++)
        // {
        //   if (Convert.ToString(data[j, i], 16).Length != 2)
        //  {
        //  tmp = tmp + " 0" + Convert.ToString(data[j, i], 16);
        // }
        // else
        // {
        // tmp = tmp + " " + Convert.ToString(data[j, i], 16);
        //}
        //}

        // tmp = tmp + "\n";
        //}

        //l.Text = tmp;
        //}
        public string show()
        {
            string tmp = "";

            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    if (Convert.ToString(data[j, i], 16).Length != 2)
                    {
                        tmp = tmp + " 0" + Convert.ToString(data[j, i], 16);
                    }
                    else
                    {
                        tmp = tmp + " " + Convert.ToString(data[j, i], 16);
                    }
                }

                tmp = tmp + "\n";
            }

            return tmp;
        }
        public string tostring()
        {
            string tmp = "";

            for (int i = 0; i < this.cols; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Convert.ToString(this.data[j, i], 16).Length != 2)
                    {
                        tmp = tmp + "0" + Convert.ToString(this.data[j, i], 16);
                    }
                    else
                    {
                        tmp = tmp + Convert.ToString(this.data[j, i], 16);
                    }
                }
            }
            return tmp;
        }

        byte[,] Kl;
        byte[,] Kw;

        public byte[,] random()
        {

            Random rand = new Random();
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    {
                        this.data[i, j] = Convert.ToByte(rand.Next(0, 256));
                    }
                }
            }

            byte[,] randomm = this.data;

            //byte [,] bbb = {{0,8},{1,9},{2,10},{3,11},{4,12},{5,13},{6,14},{7,15},{7,15}};
            //this.data = bbb;


            return randomm;

        }
        public byte[,] rozbuv(int keyl, int blokl)
        {
            if (keyl == blokl)
            {
                Kl = data;
                Kw = Kl;
            }
            else
            {

                Kl = new byte[8, cols / 2];
                Kw = new byte[8, cols / 2];
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (j < cols / 2)
                        {
                            Kl[i, j] = this.data[i, j];
                        }
                        else
                        {
                            Kw[i, j - (cols / 2)] = this.data[i, j];
                        }

                    }
                }
            }

            return this.data;
        }
        public string cyclic_shift(Char[] arr, int n)
        {
            Char[] shifted_array = new Char[arr.Length];

            string result = "";

            for (int i = n, k = 0; i < arr.Length; i++, k++)
            {
                shifted_array[i] = arr[k];
            }

            for (int i = 0, k = arr.Length - n; i < n; i++, k++)
            {
                shifted_array[i] = arr[k];
            }

            for (int i = 0; i < shifted_array.Length; i++)
            {
                result += shifted_array[i];
            }

            return result;
        }
        public Byte[] two_dim_to_one_dim(int data_length)
        {
            Byte[] one_dim_array = new Byte[data_length / 8];

            int counter = 0;

            for (int cols = 0; cols < data_length / 64; cols++)
            {
                for (int rows = 0; rows < 8; rows++, counter++)
                {
                    one_dim_array[counter] = this.data[rows, cols];
                }
            }

            return one_dim_array;
        }


        /*public State[] key_expanding(int data_length, int key_length)
        {
            #region constants

            string[] const_128 = new string[3];
            string[] const_256 = new string[4];
            string[] const_512 = new string[8];


            ////////
            const_128[0] = "11111111111111101111111111111111";
            const_128[1] = "22222222222222202222222222222221";
            const_128[2] = "33333333333333303333333333333331";

            const_256[0] = "1111111111111110111111111111111111111111111111121111111111111113";
            const_256[1] = "2222222222222220222222222222222122222222222222222222222222222223";
            const_256[2] = "3333333333333330333333333333333133333333333333323333333333333333";
            const_256[3] = "4444444444444440444444444444444144444444444444424444444444444443";

            const_512[0] = "11111111111111101111111111111111111111111111111211111111111111131111111111111114111111111111111511111111111111161111111111111117";
            const_512[1] = "22222222222222202222222222222221222222222222222222222222222222232222222222222224222222222222222522222222222222262222222222222227";
            const_512[2] = "33333333333333303333333333333331333333333333333233333333333333333333333333333334333333333333333533333333333333363333333333333337";
            const_512[3] = "44444444444444404444444444444441444444444444444244444444444444434444444444444444444444444444444544444444444444464444444444444447";
            const_512[4] = "55555555555555505555555555555551555555555555555255555555555555535555555555555554555555555555555555555555555555565555555555555557";
            const_512[5] = "66666666666666606666666666666661666666666666666266666666666666636666666666666664666666666666666566666666666666666666666666666667";
            const_512[6] = "77777777777777707777777777777771777777777777777277777777777777737777777777777774777777777777777577777777777777767777777777777777";
            const_512[7] = "88888888888888808888888888888881888888888888888288888888888888838888888888888884888888888888888588888888888888868888888888888887";
            /////////////////

            #endregion

            #region key_states_forming

            #region key_states_choose

            int iterations = 0;

            if (data_length == 128)
            {
                if (key_length == 128) { iterations = 3; }
                else { iterations = 2; }
            }
            if (data_length == 256)
            {
                iterations = 4;
            }
            if (data_length == 512)
            {
                iterations = 8;
            }

            #endregion

            keys_exp_values = new Dictionary<string, State>();

            State[] key_states = new State[iterations];
            State inverted = this.invert(); this.keys_exp_values.Add("inverted_key", inverted);
            for (int i = 0; i < iterations; i++)
            {
                key_states[i] = new State(this.cols * 64);

                if (this.cols == 2)
                {
                    key_states[i].add_hex(const_128[i]);
                }
                if (this.cols == 4)
                {
                    key_states[i].add_hex(const_256[i]);
                }
                if (this.cols == 8)
                {
                    key_states[i].add_hex(const_512[i]);
                }

                                                         this.keys_exp_values.Add("const" + i, key_states[i]);
                #region key_state_encrypting

                key_states[i].XORRoundKey(this);         this.keys_exp_values.Add("keystate" + i + "XorRoundKey1", key_states[i]);
                key_states[i].S_boxes();                 this.keys_exp_values.Add("keystate" + i + "SBoxes1", key_states[i]);
                key_states[i].Shift_Rows();              this.keys_exp_values.Add("keystate" + i + "ShiftRows1", key_states[i]);
                key_states[i].Mix_Columns();             this.keys_exp_values.Add("keystate" + i + "MixColumns1", key_states[i]);
                key_states[i].Add32RoundKey(inverted);   this.keys_exp_values.Add("keystate" + i + "Add32Roundkey", key_states[i]);
                key_states[i].S_boxes();                 this.keys_exp_values.Add("keystate" + i + "SBoxes2", key_states[i]);
                key_states[i].Shift_Rows();              this.keys_exp_values.Add("keystate" + i + "ShiftRows2", key_states[i]);
                key_states[i].Mix_Columns();             this.keys_exp_values.Add("keystate" + i + "MixColumns2", key_states[i]);
                key_states[i].XORRoundKey(this);         this.keys_exp_values.Add("keystate" + i + "XorRoundKey2", key_states[i]);

                #endregion
            }
            
            #endregion

            State[] divided_states;
            divided_states = null;
            int key_number = 16;

            #region data_128
            if (data_length == 128)
            {
                if (key_length == 128)
                {
                    key_number = 12;
                    divided_states = new State[12];

                    for (int i = 0; i < 12; i++)
                    {
                        divided_states[i] = new State(data_length);
                        divided_states[i] = null;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        divided_states[i * 4] = key_states[i];
                    }
                }

                if (key_length == 256)
                {
                    key_number = 16;
                    divided_states = new State[16];

                    for (int i = 0; i < 16; i++)
                    {
                        divided_states[i] = new State(data_length);
                        divided_states[i] = null;

                    }

                    for (int i = 0; i < 2; i++)
                    {
                        divided_states[i * 8] = key_states[i].add_part(data_length, 0);
                        divided_states[4 * (2 * i + 1)] = key_states[i].add_part(data_length, 2);

                    }
                }

                if (key_length == 512)
                {
                    key_number = 20;
                    divided_states = new State[20];

                    for (int i = 0; i < 20; i++)
                    {
                        divided_states[i] = new State(data_length);
                        divided_states[i] = null;

                    }

                    for (int i = 0; i < 2; i++)
                    {
                        divided_states[i * 16] = key_states[i].add_part(data_length, 0);
                        divided_states[4 * (4 * i + 1)] = key_states[i].add_part(data_length, 2);
                        divided_states[8 * (2 * i + 1)] = key_states[i].add_part(data_length, 4);
                        divided_states[4 * (4 * i + 3)] = key_states[i].add_part(data_length, 6);
                    }
                }
            }
            #endregion
            #region data_256
            if (data_length == 256)
            {
                if (key_length == 256)
                {
                    key_number = 16;
                    divided_states = new State[16];

                    for (int i = 0; i < 16; i++)
                    {
                        divided_states[i] = new State(data_length);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        divided_states[i * 4] = key_states[i];
                    }
                }

                if (key_length == 512)
                {
                    key_number = 32;
                    divided_states = new State[32];

                    for (int i = 0; i < 32; i++)
                    {
                        divided_states[i] = new State(key_length);
                        divided_states[i] = null;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        divided_states[i * 8] = key_states[i].add_part(data_length, 0);
                        divided_states[4 * (2 * i + 1)] = key_states[i].add_part(data_length, 4);
                    }

                }
            }
            #endregion
            #region data_512
            if (data_length == 512)
            {
                divided_states = new State[32];
                key_number = 32;

                for (int i = 0; i < 32; i++)
                {
                    divided_states[i] = new State(data_length);
                }

                for (int i = 0; i < 8; i++)
                {
                    divided_states[i * 4] = key_states[i];
                }
            }
            #endregion

            State[] states = new State[key_number];

            for (int i = 0; i < key_number / 4; i++)
            {
                Byte[] one_dim_array = new Byte[data_length / 8];

                int counter = 0;

                for (int cols = 0; cols < data_length / 64; cols++)
                {
                    for (int rows = 0; rows < 8; rows++, counter++)
                    {
                        one_dim_array[counter] = divided_states[4 * i].data[rows, cols];
                    }
                }

                BigInteger temp = new BigInteger(one_dim_array, data_length / 8);

                string str = temp.ToString(2);

                while (str.Length != data_length)
                {
                    str = "0" + str;
                }

                Char[] array = str.ToCharArray();

                int a = 11, b = 17, c = 29;

                if (data_length == 128)
                {
                    a = 5; b = 7; c = 11;
                }

                if (data_length == 265)
                {
                    a = 11; b = 17; c = 29;
                }

                if (data_length == 512)
                {
                    a = 17; b = 31; c = 47;
                }

                states[4 * i] = divided_states[4 * i];

                string tmp = this.cyclic_shift(array, a);
                BigInteger temp_bigint = new BigInteger(tmp, 2);
                states[4 * i + 1] = new State(data_length);
                states[4 * i + 1].add_hex(temp_bigint.ToHexString());

                tmp = this.cyclic_shift(array, b);
                temp_bigint = new BigInteger(tmp, 2);
                states[4 * i + 2] = new State(data_length);
                states[4 * i + 2].add_hex(temp_bigint.ToHexString());

                tmp = this.cyclic_shift(array, c);
                temp_bigint = new BigInteger(tmp, 2);
                states[4 * i + 3] = new State(data_length);
                states[4 * i + 3].add_hex(temp_bigint.ToHexString());
            }

            return states;
        }

        public void Encrypt(State[] subkeys, int key_length)
        {
            int Nr = 10;

            if (key_length == 128)
            { Nr = 10; }
            if (key_length == 256)
            { Nr = 14; }
            if (key_length == 512)
            { Nr = 18; }

            #region procedure
                                                            

            this.Add32RoundKey(subkeys[0]);                                   

            for (int round = 1; round <= Nr - 1; round++)
            {                                                    
                this.S_boxes();                                    
                this.Shift_Rows();                                 
                this.Mix_Columns();                                 
                this.XORRoundKey(subkeys[round - 1]);         
            }
            this.S_boxes(); 
            this.Shift_Rows();                                  
            this.Mix_Columns();
            this.Add32RoundKey(subkeys[Nr]); 
            #endregion

        }
        */
        public State Decrypt(State[] subkeys, int key_length)
        {
            int Nr = 10;

            if (key_length == 128)
            { Nr = 10; }
            if (key_length == 256)
            { Nr = 14; }
            if (key_length == 512)
            { Nr = 18; }

            #region procedure

            this.Sub32RoundKey(subkeys[Nr]);
            this.Inv_MixColumns();
            this.Inv_Shift_Rows();
            this.Inv_S_boxes();

            for (int round = Nr - 1; round >= 1; round--)
            {

                this.XORRoundKey(subkeys[round - 1]);
                this.Inv_MixColumns();
                this.Inv_Shift_Rows();
                this.Inv_S_boxes();
            }

            Sub32RoundKey(subkeys[0]);
            #endregion

            return this;

        }

        byte[,] _128_128 = { { 5, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };

        byte[,] _128_256 = { { 7, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };

        byte[,] _256_256 = { { 9, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } ,
                           { 0, 0,0,0 },{ 0, 0,0,0 },{ 0, 0,0,0 }};

        byte[,] _256_512 = { { 13, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } 
                           , { 0, 0,0,0 }, { 0, 0,0,0 }, { 0, 0,0,0 }};
        byte[,] _512_512 = { { 17, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } 
                           , { 0, 0,0,0,0,0,0,0 }, { 0, 0,0,0,0,0,0,0 }, { 0, 0,0,0,0,0,0,0 }
                           , { 0, 0,0,0,0,0,0,0 }, { 0, 0,0,0,0,0,0,0 }};
        byte[,] b;
        public byte[,] firstSum(int keyl, int blokl) //1
        {
            byte[,] buff = new byte[8, cols];
            if (keyl == 128 && blokl == 128)
            {
                buff = _128_128;
            }
            else if (keyl == 256 && blokl == 128)
            {
                buff = _128_256;
                cols = 2;
            }
            else if (keyl == 256 && blokl == 256)
            {
                buff = _256_256;
            }
            else if (keyl == 512 && blokl == 256)
            {
                buff = _256_512;
                cols = 4;
            }
            else if (keyl == 512 && blokl == 512)
            {
                buff = _512_512;
            }

            b = new byte[8, cols];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    b[i, j] = Convert.ToByte((Convert.ToInt32(Kl[i, j]) + Convert.ToInt32(buff[i, j])) % 255);
                }
            }

            this.data = b;
            return b;
        }

        public void XOR()
        {
            for (int j = 0; j < this.cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    this.data[i, j] = Convert.ToByte(this.data[i, j] ^ Kw[i, j]);
                }
            }
        }

        public void change()
        {

            byte[,] result = new byte[8, cols];

            for (int j = 0; j < cols; j++)
            {
                int over = 0;
                for (int i = 0; i < 8; i++)
                {
                    int variable = this.data[i, j] + Kl[i, j] + over;
                    if (variable > 255)
                    {
                        over = 1;
                        variable = variable - 256;
                    }
                    else
                    {
                        over = 0;
                    }
                    result[i, j] = (byte)variable;
                }
            }
            this.data = result;
        }

        public string[] key_forming(int blokl, int keyl)
        {
            string[] tmv_128_128 = new string[6];
            string[] tmv_128_256 = new string[8];
            string[] tmv_256_256 = new string[8];
            string[] tmv_256_512 = new string[10];
            string[] tmv_512_512 = new string[10];

            tmv_128_128[0] = "01000100010001000100010001000100";
            tmv_128_128[1] = "02000200020002000200020002000200";
            tmv_128_128[2] = "04000400040004000400040004000400";
            tmv_128_128[3] = "08000800080008000800080008000800";
            tmv_128_128[4] = "10001000100010001000100010001000";
            tmv_128_128[5] = "20002000200020002000200020002000";

            tmv_128_256[0] = "01000100010001000100010001000100";
            tmv_128_256[1] = "02000200020002000200020002000200";
            tmv_128_256[2] = "04000400040004000400040004000400";
            tmv_128_256[3] = "08000800080008000800080008000800";
            tmv_128_256[4] = "10001000100010001000100010001000";
            tmv_128_256[5] = "20002000200020002000200020002000";
            tmv_128_256[6] = "40004000400040004000400040004000";
            tmv_128_256[7] = "80008000800080008000800080008000";

            tmv_256_256[0] = "0100010001000100010001000100010001000100010001000100010001000100";
            tmv_256_256[1] = "0200020002000200020002000200020002000200020002000200020002000200";
            tmv_256_256[2] = "0400040004000400040004000400040004000400040004000400040004000400";
            tmv_256_256[3] = "0800080008000800080008000800080008000800080008000800080008000800";
            tmv_256_256[4] = "1000100010001000100010001000100010001000100010001000100010001000";
            tmv_256_256[5] = "2000200020002000200020002000200020002000200020002000200020002000";
            tmv_256_256[6] = "4000400040004000400040004000400040004000400040004000400040004000";
            tmv_256_256[7] = "8000800080008000800080008000800080008000800080008000800080008000";

            tmv_256_512[0] = "0100010001000100010001000100010001000100010001000100010001000100";
            tmv_256_512[1] = "0200020002000200020002000200020002000200020002000200020002000200";
            tmv_256_512[2] = "0400040004000400040004000400040004000400040004000400040004000400";
            tmv_256_512[3] = "0800080008000800080008000800080008000800080008000800080008000800";
            tmv_256_512[4] = "1000100010001000100010001000100010001000100010001000100010001000";
            tmv_256_512[5] = "2000200020002000200020002000200020002000200020002000200020002000";
            tmv_256_512[6] = "4000400040004000400040004000400040004000400040004000400040004000";
            tmv_256_512[7] = "8000800080008000800080008000800080008000800080008000800080008000";
            tmv_256_512[8] = "0001000100010001000100010001000100010001000100010001000100010001";
            tmv_256_512[9] = "0002000200020002000200020002000200020002000200020002000200020002";

            tmv_512_512[0] = "01000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100";
            tmv_512_512[1] = "02000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200";
            tmv_512_512[2] = "04000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400";
            tmv_512_512[3] = "08000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800";
            tmv_512_512[4] = "10001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000";
            tmv_512_512[5] = "20002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000";
            tmv_512_512[6] = "40004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000400040004000";
            tmv_512_512[7] = "80008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000800080008000";
            tmv_512_512[8] = "00010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001000100010001";
            tmv_512_512[9] = "00020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002000200020002";
            if (keyl == 128 && blokl == 128)
            {
                return tmv_128_128;
            }
            else if (keyl == 256 && blokl == 128 || keyl == 128 && blokl == 256)
            {
                return tmv_128_256;
            }
            else if (keyl == 256 && blokl == 256)
            {
                return tmv_256_256;
            }
            else if (keyl == 512 && blokl == 256 || keyl == 256 && blokl == 512)
            {
                return tmv_256_512;
            }
            else if (keyl == 512 && blokl == 512)
            {
                return tmv_512_512;
            }
            else
            { return tmv_128_256; }
        }


        public byte[,] toHex(string datastr, int var_key_length)
        {
            int cols1 = 0;
            switch (var_key_length)
            {
                case 128: cols1 = 2;
                    break;
                case 256: cols1 = 4;
                    break;
                case 512: cols1 = 8;
                    break;
            }

            numHex nh = new numHex();
            int start = 0;
            byte[,] b = new byte[8, cols1];
            for (int j = 0; j < cols1; j++)
            {
                for (int i = 0; i < 8; i++)
                {

                    b[i, j] = (byte)nh.ConvertInt(datastr.Substring(start, 2));
                    start = start + 2;

                }
            }

            return b;
        }

        byte[] rightShift;
        byte[,] rightShiftFinal;
        public byte[,] zsuvv(byte[,] rand, int zsuv, int con, int cols2)
        {
            rightShift = new byte[rand.Length];
            rightShiftFinal = new byte[8, cols2];

            byte[] buf = new byte[rand.Length];
            int c_buf = 0;
            for (int j = 0; j < cols2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    buf[c_buf] = rand[i, j];
                    c_buf++;
                }
            }

            int shiftCount = 0;
            foreach (byte b in rand)
            {
                rightShift[(shiftCount + ((zsuv / 8) * con) * 7) % rand.Length] = buf[shiftCount];
                shiftCount++;
            }

            shiftCount = 0;
            for (int j = 0; j < cols2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    rightShiftFinal[i, j] = rightShift[shiftCount];
                    shiftCount++;
                }
            }
            return rightShiftFinal;
        }

        public byte[,] subNumbers(byte[,] number1, byte[,] number2, int cols)
        {
            /* byte[] buf1 = new byte[number1.Length];
             byte[] buf2 = new byte[number2.Length];
             byte[] buf3 = new byte[number2.Length];
             byte[,] result = new byte[8, cols];
             int c_buf = 0;
             for (int j = 0; j < cols; j++)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     buf1[c_buf] = number1[i, j];
                     buf2[c_buf] = number2[i, j];
                     c_buf++;
                 }
             }

             int over = 0;
             for (int i = number1.Length - 1; i >= 0; i--)
             {
                 int variable;

                 if (buf1[i] < buf2[i])
                 {
                     variable = buf1[i] - buf2[i] + 256 - over;
                 }
                 else
                 {
                     variable = buf1[i] - buf2[i];
                 }

                 if (variable + buf2[i] > 255)
                 {
                     over = 1;
                 }
                 else
                 {
                     over = 0;
                 }
                 buf3[i] = (byte)variable;


             }

             c_buf = 0;
             for (int j = 0; j < cols; j++)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     result[i, j] = buf3[c_buf];
                     c_buf++;
                 }
             }
             */

            byte[,] result = new byte[8, cols];

            for (int j = 0; j < cols; j++)
            {
                numHex nh = new numHex();
                string s = "";
                string v = "";
                for (int i = 7; i >= 0; i--)
                {
                    string _1 = "0000000" + Convert.ToString(number1[i, j], 2);
                    s += _1.Substring(_1.Length - 8, 8);
                    string _2 = "0000000" + Convert.ToString(number2[i, j], 2);
                    v += _2.Substring(_2.Length - 8, 8);
                }

                long n1 = Convert.ToInt64(s, 2);
                long n2 = Convert.ToInt64(v, 2);

                long nn = n1 - n2;

                s = "";
                s = "000000000000000000000000000000000000000000000000000000000000000" + Convert.ToString(nn, 2);
                s = s.Substring(s.Length - 64, 64);

                int co = 0;
                for (int i = 7; i >= 0; i--)
                {
                    result[i, j] = (byte)Convert.ToUInt32(s.Substring(co, 8), 2);
                    co += 8;
                }
            }

            return result;
        }

        public byte[,] addNumbers(byte[,] number1, byte[,] number2, int cols)
        {
            /* byte[,] result = new byte[8, cols];

             for (int j = 0; j < cols; j++)
             {
                 int over = 0;
                 for (int i = 0; i < 8; i++)
                 {
                     int variable = number1[i, j] + number2[i, j] + over;
                     if (variable > 255)
                     {
                         over = 1;
                         variable = variable - 256;
                     }
                     else
                     {
                         over = 0;
                     }
                     result[i, j] = (byte)variable;
                 }
             }*/

            byte[,] result = new byte[8, cols];

            for (int j = 0; j < cols; j++)
            {
                numHex nh = new numHex();
                string s = "";
                string v = "";
                for (int i = 7; i >= 0; i--)
                {
                    string _1 = "0000000" + Convert.ToString(number1[i, j], 2);
                    s += _1.Substring(_1.Length - 8, 8);
                    string _2 = "0000000" + Convert.ToString(number2[i, j], 2);
                    v += _2.Substring(_2.Length - 8, 8);
                }

                long n1 = Convert.ToInt64(s, 2);
                long n2 = Convert.ToInt64(v, 2);

                long nn = n1 + n2;

                s = "";
                s = "000000000000000000000000000000000000000000000000000000000000000" + Convert.ToString(nn, 2);
                s = s.Substring(s.Length - 64, 64);

                int co = 0;
                for (int i = 7; i >= 0; i--)
                {
                    result[i, j] = (byte)Convert.ToUInt32(s.Substring(co, 8), 2);
                    co += 8;
                }
            }

            return result;
        }

        /*  public byte[,] zzz (byte[,] number1, byte[,] number2, int cols)
          {
              //byte[] buf1 = new byte[number1.Length];
              //byte[] buf2 = new byte[number2.Length];
              //byte[] buf3 = new byte[number2.Length];
              int buf3;
              byte[,] result = new byte[8, cols];
              int c_buf = 0;
             /*for (int j = 0; j < cols; j++)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     buf1[c_buf] = number1[i, j];
                     buf2[c_buf] = number2[i, j];
                     c_buf++;
                 }
             }

             for (int j = 0; j < cols; j++)
             {
                 int over = 0;
                 for (int i = 0; i < 8; i++)
                 {
                     int variable = number1[i,j] + number2[i, j] + over;
                     if (variable > 255)
                     {
                         over = 1;
                         variable = variable - 256;
                     }
                     else
                     {
                         over = 0;
                     }
                     result[i, j] = (byte)variable;
                 }
             }
                    
              for (int i = 0; i < number1.Length; i++)
              {
                 
                  if (i != 0)
                  {
                     
                      if (variable > 255)
                      {
                          over = 1;
                          variable = variable - 256;
                      }
                      else
                      {
                          over = 0;
                      }
                      buf3[i] = (byte)variable;
                  }
                  else
                  {
                      buf3[i] = (byte)over;
                  }
              }

              c_buf = 0;
              for (int j = 0; j < cols; j++)
              {
                  for (int i = 0; i < 8; i++)
                  {
                      result[i, j] = buf3[c_buf];
                      c_buf++;
                  }
              }
              return result;
          }
         */

        public byte[,] sBoxes(byte[,] kt_raund, int cols1)
        {
            string[] sbox_value = new string[8];

            sbox_value[0] = "a86d3e92dc2e34229beb78b32ff7ac8143f3dd72f2e748334c8411beefea52545f1da39e83e2fc24fe6901cee80a64c006cb4f61375ab728603ae5bd8c0d10ed6bc9b4d142966a365c9d00ae357ed04e754db663e41688c7dad768e903f8d9446c2c9afa7a23a5b218d3988ad45013a759af0eee322b533b4670a0317f1a0c2a71791ff49cc2868ecd67c51cfbc41285dfe0bf19cc65f9777d4002ec05072925879715d5ab665bba21b5a6f1c15751e695fde1ad4a0fdbf5b0de74995eb8b9ca176f49588fbc38143f5d2d94903ccf7cf04bd2a46ea97b9f1b300baa2062d68bd84593bb0447c3088991a2f63de373560939c6a127411e55ffb1762682c88d80";
            sbox_value[1] = "ce42317c1a4ce7302a7d188bf15072b6bb15f6266b632bdcda9332466e7a2cc2eb56642e698e48b7c91b7180942fdd0192b458f7f35bfd6c00fe4b1e2874d0f0ea659e08bdcc964a7ec4ef389a53875acb1cf45d333c45b5a2473be184b3beed13882244ab19fc3f550970b8e8615ea7c143aa3efaa14197bf86a0a8a3afa666e9c5759fd18112d4110be4e04f39ec213a5c0f149b490d62d58f400c7735047fd63602c8687b792d9c9dff23d3dec68ab2bab1ae4ed9e506cf6ac37685cd0327d2f5df54166f89a40e07a91de21f34c790576d1095378ca50ab9e6255299fbc0176773d89160e3833db07824f2acdb29f88d4dbceeca205f5198f90582ad59d7";
            sbox_value[2] = "934a49a62510561e8727b373526bf642d9173c2ca5d5810bc9cf69f9b6f5ff049a2bc0c4d74f8f05f09957ca087913a0b5c2d8e3039e77d65da81f3af3bf58db98945c76114dcc146d50071aae01f13922f49b7800a99c6e3f0f1cfbbe5f478645bbadb7c355b96c88378a0d19750a54fca385b42ec6e27e8d24bcc189637faaba62530992d0ac66c72820fe321bc58c6ae4a13bef7bb8fdf730ebfa2623a734df717a0e4e182fb11d95cef2b03de72102d4c841129715e5e9d28e6fea68618b9fcd2d4c9dd3a460ec3eabbd4b2a5af8dc70e0de7d367cafed5bee966465060c5116d1b2cbe6da5e804031dd84e8467459e17290354838332983a24382914467";
            sbox_value[3] = "68225870b53172ea34c3976ca7e7cb648d032f5d381444bc7e9f2e90c6b0bb6dca460df36eae156210b6f80093826bdc4d3d02450eeefd0cf1d7658e0ff776f0732ded40e5c837247b29f66f0afeba594b4a51ccf448bea68fc27550069d5aa94e539ee8f9d35fa863eb0701e6877d4c2a8311948630aaeca0c004c52b5c7817d413f256e9a19b6705a449da96810b7f528a3e084f9288209a8b3347a335959126b755ced641d8db438ce43f1cdee3b8b3d55e1a85b1ab7c771dd9cdafb4adc95425d13a2318892821fbb9696aa574571e7916d2cfc49cddbfffd0a212fc981b19f53ce1322cfaac27c142e284803be01fbd66df9971605b09b2c77a39ef3661";
            sbox_value[4] = "a86d3e92dc2e34229beb78b32ff7ac8143f3dd72f2e748334c8411beefea52545f1da39e83e2fc24fe6901cee80a64c006cb4f61375ab728603ae5bd8c0d10ed6bc9b4d142966a365c9d00ae357ed04e754db663e41688c7dad768e903f8d9446c2c9afa7a23a5b218d3988ad45013a759af0eee322b533b4670a0317f1a0c2a71791ff49cc2868ecd67c51cfbc41285dfe0bf19cc65f9777d4002ec05072925879715d5ab665bba21b5a6f1c15751e695fde1ad4a0fdbf5b0de74995eb8b9ca176f49588fbc38143f5d2d94903ccf7cf04bd2a46ea97b9f1b300baa2062d68bd84593bb0447c3088991a2f63de373560939c6a127411e55ffb1762682c88d80";
            sbox_value[5] = "ce42317c1a4ce7302a7d188bf15072b6bb15f6266b632bdcda9332466e7a2cc2eb56642e698e48b7c91b7180942fdd0192b458f7f35bfd6c00fe4b1e2874d0f0ea659e08bdcc964a7ec4ef389a53875acb1cf45d333c45b5a2473be184b3beed13882244ab19fc3f550970b8e8615ea7c143aa3efaa14197bf86a0a8a3afa666e9c5759fd18112d4110be4e04f39ec213a5c0f149b490d62d58f400c7735047fd63602c8687b792d9c9dff23d3dec68ab2bab1ae4ed9e506cf6ac37685cd0327d2f5df54166f89a40e07a91de21f34c790576d1095378ca50ab9e6255299fbc0176773d89160e3833db07824f2acdb29f88d4dbceeca205f5198f90582ad59d7";
            sbox_value[6] = "934a49a62510561e8727b373526bf642d9173c2ca5d5810bc9cf69f9b6f5ff049a2bc0c4d74f8f05f09957ca087913a0b5c2d8e3039e77d65da81f3af3bf58db98945c76114dcc146d50071aae01f13922f49b7800a99c6e3f0f1cfbbe5f478645bbadb7c355b96c88378a0d19750a54fca385b42ec6e27e8d24bcc189637faaba62530992d0ac66c72820fe321bc58c6ae4a13bef7bb8fdf730ebfa2623a734df717a0e4e182fb11d95cef2b03de72102d4c841129715e5e9d28e6fea68618b9fcd2d4c9dd3a460ec3eabbd4b2a5af8dc70e0de7d367cafed5bee966465060c5116d1b2cbe6da5e804031dd84e8467459e17290354838332983a24382914467";
            sbox_value[7] = "68225870b53172ea34c3976ca7e7cb648d032f5d381444bc7e9f2e90c6b0bb6dca460df36eae156210b6f80093826bdc4d3d02450eeefd0cf1d7658e0ff776f0732ded40e5c837247b29f66f0afeba594b4a51ccf448bea68fc27550069d5aa94e539ee8f9d35fa863eb0701e6877d4c2a8311948630aaeca0c004c52b5c7817d413f256e9a19b6705a449da96810b7f528a3e084f9288209a8b3347a335959126b755ced641d8db438ce43f1cdee3b8b3d55e1a85b1ab7c771dd9cdafb4adc95425d13a2318892821fbb9696aa574571e7916d2cfc49cddbfffd0a212fc981b19f53ce1322cfaac27c142e284803be01fbd66df9971605b09b2c77a39ef3661";

            for (int i = 0; i < 8; i++)
            {
                int start = 0;
                byte[,] sbox = new byte[16, 16];

                for (int n = 0; n < 16; n++)
                {
                    for (int m = 0; m < 16; m++)
                    {
                        if (start <= sbox_value[i].Length - 2)
                        {
                            sbox[m, n] = Convert.ToByte(sbox_value[i].Substring(start, 2), 16);
                            start = start + 2;
                        }
                    }
                }


                for (int j = 0; j < cols1; j++)
                {
                    int x = kt_raund[i, j] >> 4;
                    int y = kt_raund[i, j] & 15;
                    kt_raund[i, j] = sbox[x, y];
                }

            }

            return kt_raund;
        }

        #region s_row
        public byte[,] S_Rows(byte[,] s_box1, int cols1)
        {
            byte buffer;
            #region shift128
            if (cols1 == 2)
            {
                for (int i = 4; i < 8; i++)
                {
                    buffer = s_box1[i, 1];
                    s_box1[i, 1] = s_box1[i, 0];
                    s_box1[i, 0] = buffer;
                }
            }
            #endregion
            #region shift256
            if (cols1 == 4)
            {
                s_box1 = simple_256_shift1(s_box1, 2, 1);
                s_box1 = simple_256_shift1(s_box1, 4, 2);
                s_box1 = simple_256_shift1(s_box1, 6, 3);
            }
            #endregion
            #region shift512
            if (cols1 == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    s_box1 = simple_512_shift1(s_box1, i, i);
                }
            }
            #endregion

            return s_box1;
        }

        private byte[,] simple_512_shift1(byte[,] s_box1, int row, int num)
        {
            for (int iter = 0; iter < num; iter++)
            {
                byte buffer = s_box1[row, 7];
                s_box1[row, 7] = s_box1[row, 6];
                s_box1[row, 6] = s_box1[row, 5];
                s_box1[row, 5] = s_box1[row, 4];
                s_box1[row, 4] = s_box1[row, 3];
                s_box1[row, 3] = s_box1[row, 2];
                s_box1[row, 2] = s_box1[row, 1];
                s_box1[row, 1] = s_box1[row, 0];
                s_box1[row, 0] = buffer;
            }

            return s_box1;
        }

        private byte[,] simple_256_shift1(byte[,] s_box1, int row, int num)
        {
            for (int j = row; j < row + 2; j++)
            {
                for (int i = 0; i < num; i++)
                {
                    byte buffer = s_box1[j, 3];
                    s_box1[j, 3] = s_box1[j, 2];
                    s_box1[j, 2] = s_box1[j, 1];
                    s_box1[j, 1] = s_box1[j, 0];
                    s_box1[j, 0] = buffer;
                }
            }
            return s_box1;
        }

        #endregion


        public byte[,] M_Columns(byte[,] s_row, int cols1)
        {
            for (int j = 0; j < cols1; j++)
            {
                #region temp

                byte a = s_row[0, j];
                byte b = s_row[1, j];
                byte c = s_row[2, j];
                byte d = s_row[3, j];
                byte e = s_row[4, j];
                byte f = s_row[5, j];
                byte g = s_row[6, j];
                byte h = s_row[7, j];

                #endregion

                s_row[0, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x05, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x08, e) ^ Form1.GMul(0x06, f) ^ Form1.GMul(0x07, g) ^ Form1.GMul(0x04, h));
                s_row[1, j] = (byte)(Form1.GMul(0x04, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x05, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x08, f) ^ Form1.GMul(0x06, g) ^ Form1.GMul(0x07, h));
                s_row[2, j] = (byte)(Form1.GMul(0x07, a) ^ Form1.GMul(0x04, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x05, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x08, g) ^ Form1.GMul(0x06, h));
                s_row[3, j] = (byte)(Form1.GMul(0x06, a) ^ Form1.GMul(0x07, b) ^ Form1.GMul(0x04, c) ^ Form1.GMul(0x01, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x05, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x08, h));
                s_row[4, j] = (byte)(Form1.GMul(0x08, a) ^ Form1.GMul(0x06, b) ^ Form1.GMul(0x07, c) ^ Form1.GMul(0x04, d) ^ Form1.GMul(0x01, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x05, g) ^ Form1.GMul(0x01, h));
                s_row[5, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x08, b) ^ Form1.GMul(0x06, c) ^ Form1.GMul(0x07, d) ^ Form1.GMul(0x04, e) ^ Form1.GMul(0x01, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x05, h));
                s_row[6, j] = (byte)(Form1.GMul(0x05, a) ^ Form1.GMul(0x01, b) ^ Form1.GMul(0x08, c) ^ Form1.GMul(0x06, d) ^ Form1.GMul(0x07, e) ^ Form1.GMul(0x04, f) ^ Form1.GMul(0x01, g) ^ Form1.GMul(0x01, h));
                s_row[7, j] = (byte)(Form1.GMul(0x01, a) ^ Form1.GMul(0x05, b) ^ Form1.GMul(0x01, c) ^ Form1.GMul(0x08, d) ^ Form1.GMul(0x06, e) ^ Form1.GMul(0x07, f) ^ Form1.GMul(0x04, g) ^ Form1.GMul(0x01, h));
            }

            return s_row;

        }

        public byte[,] xor_rkey(byte[,] num1, byte[,] num2, int cols1)
        {
            byte[,] result = new byte[8, cols1];

            for (int j = 0; j < cols1; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    result[i, j] = Convert.ToByte(num1[i, j] ^ num2[i, j]);
                }
            }

            return result;
        }


        public byte[,] addNumbers1(byte[,] number1, byte[,] number2, byte[,] number3, int cols)
        {
            byte[] buf1 = new byte[number1.Length];
            byte[] buf2 = new byte[number2.Length];
            byte[] buf4 = new byte[number2.Length];
            byte[] buf3 = new byte[number2.Length];
            int buf;
            byte[,] result = new byte[8, cols];
            int c_buf = 0;

            for (int j = 0; j < cols; j++)
            {
                int over = 0;
                for (int i = 7; i >= 0; i--)
                {
                    buf = number1[i, j] + number2[i, j] + over;
                    if (buf > 255)
                    {
                        over = 1;
                        buf = buf - 256;
                    }
                    else
                    {
                        over = 0;
                    }
                    result[i, j] = (byte)buf;
                }
            }
            /*for (int j = 0; j < cols; j++)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     buf1[c_buf] = number1[i, j];
                     buf2[c_buf] = number2[i, j];
                    buf4[c_buf] = number3[i, j];
                    c_buf++;
                 }
             }
             /////////////////
             int over = 0;
             for (int i = number1.Length-1; i >= 0; i--)
             {
                 int variable;

                     variable = buf1[i] + buf2[i] + over;
                     if (variable > 255)
                     {
                         over = 1;
                         variable = variable - 256;
                     }
                     else
                     {
                         over = 0;
                     }
                     buf3[i] = (byte)variable;
                 if(i == 0)
                 {
                     buf3[number1.Length - 1] = (byte)(over+ buf3[number1.Length-1]);
                 }
             }

             c_buf = 0;
             for (int j = 0; j < cols; j++)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     result[i, j] = buf3[c_buf];
                     c_buf++;
                 }
             }
             */
            return result;
        }

        byte[] leftShift;
        byte[,] leftShiftFinal;
        public byte[,] zsuvvL(byte[,] rand, int zsuv, int con, int cols2)
        {
            leftShift = new byte[rand.Length];
            leftShiftFinal = new byte[8, cols2];

            byte[] buf = new byte[rand.Length];
            int c_buf = 0;
            for (int j = 0; j < cols2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    buf[c_buf] = rand[i, j];
                    c_buf++;
                }
            }

            int shiftCount = 0;
            foreach (byte b in rand)
            {
                int a = (shiftCount - ((zsuv / 8) * con));
                if (a < 0)
                {
                    a = a + rand.Length;
                }

                leftShift[a % rand.Length] = buf[shiftCount];
                shiftCount++;
            }

            shiftCount = 0;
            for (int j = 0; j < cols2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    leftShiftFinal[i, j] = leftShift[shiftCount];
                    shiftCount++;
                }
            }
            return leftShiftFinal;
        }


        public byte[,] Inv_Sboxes(byte[,] n1, int cols1)
        {
            string[] inv_sbox = new string[8];

            inv_sbox[0] = "a4e3cd9d99d68397ffcca78bf2e4190da2a18ab75fea3308f09e3f9fca342bbaa9e8704744e1dd31cf30ae76582d5241c5e65671016735ee422e22b06e96de114e7c7260f5f1e2ab91bc3d24d8c64538c92af9c41e7f5905f80b669aa83aa37b0355bf7487fe5aaf6854aa252fedfabed90c4f435eda98790a1af663759551d07e86736c613ca5a065a600dbdf0ec2d50f39e91f2c0792188ebb5deb14e5b569d2d7f7934b536446b626bd7afb85d136ad8d57771d6a046dfd804a3e136b90c8e7b816dc818406fcc348e05c4940b962d312acce159c1089ef943bb38821f31b276f5020f4cb4dd47832b4b1b29b37825b28c08c23021cc74c7d1729ec09c18f";
            inv_sbox[1] = "833d6f70a9d05ea6b20da79efde3b8f3f288f820678fd6a2584e572b0748b5c02a6826a110cd79e0cf03850bf10cccceeb06ba4517d4512e7e91c7d5abca6e43e939beec363c22d3c5c27c139478a825bf11bdd965861428cb4d7d7518896b1c7b4c311ab112f7bb9764e7f0ea0aad219c0efb5d951d1ec9e477f672fcff603334a0c3b462234aae169fb7b63a3ec60f9656fed859ef426a6cddac9d825b08af8d40800974f49bd1fac4271b5f810447989261a5a353415ab049460105ee02edb915e155501973306d8ade3f5471e86669bc7a8e2f352d901f9adf44dbe2f5638cb332374be6c18452243be500da4f9329dcd276c87f5cf99938d7878b2ca4aa";
            inv_sbox[2] = "4550a8999e947c1d8e3ff2ca22582d82d444faae3b0eeb1a61df297ab72e1fe40b4b05c8f0c0182fcf48af3e139b67ba43e2d97fbf28d7b09f0017a0465c33c3f17497f9eff6cdfece146c37321b1915726b404f0656ddd6279a4103e8517bd1ed1ec95dee607834f5bd30c157735ee0a4119096e5a2ff63805be9368842ea89c25a986f5fe3db35860493692b23defc38c68ff4200fa1d2c792556681018bb1e6b4dcb310ec092aa602f708b26ecbb971d81239cc9dd059fb25ac164ef3a9b5fda531213c24766df86568a7640d8c07b68a2cda5483844d874c26bc1cbe8d793a70479c4a7e7577ab53c4c5aa3dadb895a36a8552d5bbe7620c7dd3910a49e1";
            inv_sbox[3] = "b282795734b56f30deb187d197adfef3b6278c505a25ff5fd8f9585b9e2c3e8323cd104eae097260d259bd9f953dbe2811184ca98a16860417c2c90b9056ea32a7517480610cf0ecce3798db5d08aa4588621cd9332aa3a54be9dc40b71b441ec52d0aef12382fe347c87592c14ac6a4a6f78e64b9fc788bd6a0c01aaf93d0d3395c7c41552000e769ed76fa546a36a28f0e94cfa8f4bc1d6c4ff5acfbab4846c43b073c15e5ccbf198967e402b8706ee8fdc7ee057fe28499686be1e07a969c73ca5e2ef6d7b07b9a6d7e7135f277dd229b14130331f1e601d5eb1fbb7d2463430da129062b4281b32652653ada53d4c30f21ba4966b4f88591cb8d4d3fdf9d";
            inv_sbox[4] = "a4e3cd9d99d68397ffcca78bf2e4190da2a18ab75fea3308f09e3f9fca342bbaa9e8704744e1dd31cf30ae76582d5241c5e65671016735ee422e22b06e96de114e7c7260f5f1e2ab91bc3d24d8c64538c92af9c41e7f5905f80b669aa83aa37b0355bf7487fe5aaf6854aa252fedfabed90c4f435eda98790a1af663759551d07e86736c613ca5a065a600dbdf0ec2d50f39e91f2c0792188ebb5deb14e5b569d2d7f7934b536446b626bd7afb85d136ad8d57771d6a046dfd804a3e136b90c8e7b816dc818406fcc348e05c4940b962d312acce159c1089ef943bb38821f31b276f5020f4cb4dd47832b4b1b29b37825b28c08c23021cc74c7d1729ec09c18f";
            inv_sbox[5] = "833d6f70a9d05ea6b20da79efde3b8f3f288f820678fd6a2584e572b0748b5c02a6826a110cd79e0cf03850bf10cccceeb06ba4517d4512e7e91c7d5abca6e43e939beec363c22d3c5c27c139478a825bf11bdd965861428cb4d7d7518896b1c7b4c311ab112f7bb9764e7f0ea0aad219c0efb5d951d1ec9e477f672fcff603334a0c3b462234aae169fb7b63a3ec60f9656fed859ef426a6cddac9d825b08af8d40800974f49bd1fac4271b5f810447989261a5a353415ab049460105ee02edb915e155501973306d8ade3f5471e86669bc7a8e2f352d901f9adf44dbe2f5638cb332374be6c18452243be500da4f9329dcd276c87f5cf99938d7878b2ca4aa";
            inv_sbox[6] = "4550a8999e947c1d8e3ff2ca22582d82d444faae3b0eeb1a61df297ab72e1fe40b4b05c8f0c0182fcf48af3e139b67ba43e2d97fbf28d7b09f0017a0465c33c3f17497f9eff6cdfece146c37321b1915726b404f0656ddd6279a4103e8517bd1ed1ec95dee607834f5bd30c157735ee0a4119096e5a2ff63805be9368842ea89c25a986f5fe3db35860493692b23defc38c68ff4200fa1d2c792556681018bb1e6b4dcb310ec092aa602f708b26ecbb971d81239cc9dd059fb25ac164ef3a9b5fda531213c24766df86568a7640d8c07b68a2cda5483844d874c26bc1cbe8d793a70479c4a7e7577ab53c4c5aa3dadb895a36a8552d5bbe7620c7dd3910a49e1";
            inv_sbox[7] = "b282795734b56f30deb187d197adfef3b6278c505a25ff5fd8f9585b9e2c3e8323cd104eae097260d259bd9f953dbe2811184ca98a16860417c2c90b9056ea32a7517480610cf0ecce3798db5d08aa4588621cd9332aa3a54be9dc40b71b441ec52d0aef12382fe347c87592c14ac6a4a6f78e64b9fc788bd6a0c01aaf93d0d3395c7c41552000e769ed76fa546a36a28f0e94cfa8f4bc1d6c4ff5acfbab4846c43b073c15e5ccbf198967e402b8706ee8fdc7ee057fe28499686be1e07a969c73ca5e2ef6d7b07b9a6d7e7135f277dd229b14130331f1e601d5eb1fbb7d2463430da129062b4281b32652653ada53d4c30f21ba4966b4f88591cb8d4d3fdf9d";

            for (int i = 0; i < 8; i++)
            {
                int start = 0;
                byte[,] invsbox = new byte[16, 16];

                #region invsbox_forming
                for (int n = 0; n < 16; n++)
                {
                    for (int m = 0; m < 16; m++)
                    {
                        if (start <= inv_sbox[i].Length - 2)
                        {
                            invsbox[m, n] = Convert.ToByte(inv_sbox[i].Substring(start, 2), 16);
                            start = start + 2;
                        }
                    }
                }

                #endregion
                #region substitution
                for (int j = 0; j < cols1; j++)
                {
                    int x = n1[i, j] >> 4;
                    int y = n1[i, j] & 15;
                    n1[i, j] = invsbox[x, y];
                }
                #endregion
            }
            return n1;

        }

        public byte[,] Inv_S_Rows(byte[,] n1, int cols1)
        {
            byte buffer;
            #region shift128
            if (cols1 == 2)
            {
                for (int i = 4; i < 8; i++)
                {
                    buffer = n1[i, 1];
                    n1[i, 1] = n1[i, 0];
                    n1[i, 0] = buffer;
                }
            }
            #endregion

            #region shift256
            if (cols1 == 4)
            {
                n1 = simple_256_shift1(n1, 2, 3);
                n1 = simple_256_shift1(n1, 4, 2);
                n1 = simple_256_shift1(n1, 6, 1);
            }
            #endregion

            #region shift512
            if (cols1 == 8)
            {
                for (int i = 1; i < 8; i++)
                {
                    n1 = simple_512_shift1(n1, i, 8 - i);
                }
            }
            #endregion
            return n1;
        }
        public byte[,] Inv_MColumns(byte[,] n1, int cols1)
        {
            for (int j = 0; j < cols1; j++)
            {
                byte a = n1[0, j];
                byte b = n1[1, j];
                byte c = n1[2, j];
                byte d = n1[3, j];
                byte e = n1[4, j];
                byte f = n1[5, j];
                byte g = n1[6, j];
                byte h = n1[7, j];

                n1[0, j] = (byte)(Form1.GMul(0xad, a) ^ Form1.GMul(0x95, b) ^ Form1.GMul(0x76, c) ^ Form1.GMul(0xa8, d) ^ Form1.GMul(0x2f, e) ^ Form1.GMul(0x49, f) ^ Form1.GMul(0xd7, g) ^ Form1.GMul(0xca, h));
                n1[1, j] = (byte)(Form1.GMul(0xca, a) ^ Form1.GMul(0xad, b) ^ Form1.GMul(0x95, c) ^ Form1.GMul(0x76, d) ^ Form1.GMul(0xa8, e) ^ Form1.GMul(0x2f, f) ^ Form1.GMul(0x49, g) ^ Form1.GMul(0xd7, h));
                n1[2, j] = (byte)(Form1.GMul(0xd7, a) ^ Form1.GMul(0xca, b) ^ Form1.GMul(0xad, c) ^ Form1.GMul(0x95, d) ^ Form1.GMul(0x76, e) ^ Form1.GMul(0xa8, f) ^ Form1.GMul(0x2f, g) ^ Form1.GMul(0x49, h));
                n1[3, j] = (byte)(Form1.GMul(0x49, a) ^ Form1.GMul(0xd7, b) ^ Form1.GMul(0xca, c) ^ Form1.GMul(0xad, d) ^ Form1.GMul(0x95, e) ^ Form1.GMul(0x76, f) ^ Form1.GMul(0xa8, g) ^ Form1.GMul(0x2f, h));
                n1[4, j] = (byte)(Form1.GMul(0x2f, a) ^ Form1.GMul(0x49, b) ^ Form1.GMul(0xd7, c) ^ Form1.GMul(0xca, d) ^ Form1.GMul(0xad, e) ^ Form1.GMul(0x95, f) ^ Form1.GMul(0x76, g) ^ Form1.GMul(0xa8, h));
                n1[5, j] = (byte)(Form1.GMul(0xa8, a) ^ Form1.GMul(0x2f, b) ^ Form1.GMul(0x49, c) ^ Form1.GMul(0xd7, d) ^ Form1.GMul(0xca, e) ^ Form1.GMul(0xad, f) ^ Form1.GMul(0x95, g) ^ Form1.GMul(0x76, h));
                n1[6, j] = (byte)(Form1.GMul(0x76, a) ^ Form1.GMul(0xa8, b) ^ Form1.GMul(0x2f, c) ^ Form1.GMul(0x49, d) ^ Form1.GMul(0xd7, e) ^ Form1.GMul(0xca, f) ^ Form1.GMul(0xad, g) ^ Form1.GMul(0x95, h));
                n1[7, j] = (byte)(Form1.GMul(0x95, a) ^ Form1.GMul(0x76, b) ^ Form1.GMul(0xa8, c) ^ Form1.GMul(0x2f, d) ^ Form1.GMul(0x49, e) ^ Form1.GMul(0xd7, f) ^ Form1.GMul(0xca, g) ^ Form1.GMul(0xad, h));
            }
            return n1;
        }

    }

    class numHex
    {
        public numHex() { }
        string[] hmass = {     "00","01","02","03","04","05","06","07","08","09","0a","0b","0c","0d","0e","0f",//
                               "10","11","12","13","14","15","16","17","18","19","1a","1b","1c","1d","1e","1f",//
                               "20","21","22","23","24","25","26","27","28","29","2a","2b","2c","2d","2e","2f",//
                               "30","31","32","33","34","35","36","37","38","39","3a","3b","3c","3d","3e","3f",//
                               "40","41","42","43","44","45","46","47","48","49","4a","4b","4c","4d","4e","4f",//
                               "50","51","52","53","54","55","56","57","58","59","5a","5b","5c","5d","5e","5f",//
                               "60","61","62","63","64","65","66","67","68","69","6a","6b","6c","6d","6e","6f",//
                               "70","71","72","73","74","75","76","77","78","79","7a","7b","7c","7d","7e","7f",//
                               "80","81","82","83","84","85","86","87","88","89","8a","8b","8c","8d","8e","8f",
                               "90","91","92","93","94","95","96","97","98","99","9a","9b","9c","9d","9e","9f",
                               "a0","a1","a2","a3","a4","a5","a6","a7","a8","a9","aa","ab","ac","ad","ae","af",
                               "b0","b1","b2","b3","b4","b5","b6","b7","b8","b9","ba","bb","bc","bd","be","bf",
                               "c0","c1","c2","c3","c4","c5","c6","c7","c8","c9","ca","cb","cc","cd","ce","cf",
                               "d0","d1","d2","d3","d4","d5","d6","d7","d8","d9","da","db","dc","dd","de","df",
                               "e0","e1","e2","e3","e4","e5","e6","e7","e8","e9","ea","eb","ec","ed","ee","ef",
                               "f0","f1","f2","f3","f4","f5","f6","f7","f8","f9","fa","fb","fc","fd","fe","ff",};
        public string Convert(string s)
        {

            int i;
            for (i = 0; i < hmass.Length; i++)
            {
                if (hmass[i] == s)
                {
                    break;
                }
            }
            return i.ToString();
        }
        public string returnSt(int s)
        {
            return hmass[s];
        }
        public int ConvertInt(string s)
        {

            int i;
            for (i = 0; i < hmass.Length; i++)
            {
                if (hmass[i] == s)
                {
                    break;
                }
            }
            return i;
        }
    }
}
