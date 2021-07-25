using System;
using System.ComponentModel;
using System.Media;
using System.Security;
using System.Security.Permissions;

namespace hb.wpf
{
    /// <summary>
    /// Wav声音播放辅助类
    /// </summary>
    [HostProtection(SecurityAction.LinkDemand, Resources = HostProtectionResource.ExternalProcessMgmt)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class WavPlayer
    {
        /// <summary>
        /// 指示如何调用音频方法时，播放声音。
        /// </summary>
        public enum AudioPlayMode
        {
            /// <summary>
            /// 播放声音，并等待，直到它完成之前调用代码继续。
            /// </summary>
            WaitToComplete,

            /// <summary>
            /// 在后台播放声音。调用代码继续执行。
            /// </summary>
            Background,

            /// <summary>
            /// 直到stop方法被称为播放背景声音。调用代码继续执行。
            /// </summary>
            BackgroundLoop
        }

        private static SoundPlayer _soundPlayer;

        #region Methods

        private static void InternalStop(SoundPlayer sound)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            try
            {
                sound.Stop();
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>播放。wav声音文件。</summary>
        /// <param name="location">String，包含声音文件的名称 </param>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlThread" /><IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        public static void Play(string location)
        {
            Play(location, AudioPlayMode.Background);
        }

        /// <summary>
        /// 播放。wav声音文件.
        /// </summary>
        /// <param name="location">AudioPlayMode枚举模式播放声音。默认情况下，AudioPlayMode.Background。</param>
        /// <param name="playMode">String，包含声音文件的名称</param>
        public static void Play(string location, AudioPlayMode playMode)
        {
            ValidateAudioPlayModeEnum(playMode, "playMode");
            string text1 = ValidateFilename(location);
            SoundPlayer player1 = new SoundPlayer(text1);
            Play(player1, playMode);
        }

        private static void Play(SoundPlayer sound, AudioPlayMode mode)
        {
            if (_soundPlayer != null)
            {
                InternalStop(_soundPlayer);
            }

            _soundPlayer = sound;
            switch (mode)
            {
                case AudioPlayMode.WaitToComplete:
                    _soundPlayer.PlaySync();
                    return;

                case AudioPlayMode.Background:
                    _soundPlayer.Play();
                    return;

                case AudioPlayMode.BackgroundLoop:
                    _soundPlayer.PlayLooping();
                    return;
            }
        }

        /// <summary>
        /// 播放系统声音。
        /// </summary>
        /// <param name="systemSound">对象代表系统播放声音。</param>
        public static void PlaySystemSound(SystemSound systemSound)
        {
            if (systemSound == null)
            {
                throw new ArgumentNullException();
            }

            systemSound.Play();
        }

        /// <summary>
        /// 停止在后台播放声音。
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public static void Stop()
        {
            SoundPlayer player1 = new SoundPlayer();
            InternalStop(player1);
        }

        private static void ValidateAudioPlayModeEnum(AudioPlayMode value, string paramName)
        {
            if ((value < AudioPlayMode.WaitToComplete) || (value > AudioPlayMode.BackgroundLoop))
            {
                throw new InvalidEnumArgumentException(paramName, (int)value, typeof(AudioPlayMode));
            }
        }

        private static string ValidateFilename(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException();
            }

            return location;
        }

        #endregion Methods
    }

}
