-- phpMyAdmin SQL Dump
-- version 4.6.5.2
-- https://www.phpmyadmin.net/
--
-- 主機: 127.0.0.1:4406
-- 產生時間： 2017-03-02 07:17:05
-- 伺服器版本: 5.5.28
-- PHP 版本： 7.1.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 資料庫： `dnake`
--

-- --------------------------------------------------------

--
-- 資料表結構 `access_auth`
--

CREATE TABLE `access_auth` (
  `id` int(11) NOT NULL,
  `card` varchar(64) NOT NULL,
  `user` varchar(64) NOT NULL,
  `auth` int(11) DEFAULT '0',
  `done` int(11) DEFAULT '0',
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `access_card`
--

CREATE TABLE `access_card` (
  `id` int(11) NOT NULL,
  `card` varchar(64) NOT NULL,
  `gid` int(11) DEFAULT '0',
  `name` varchar(64) DEFAULT NULL,
  `build` int(11) DEFAULT '0',
  `unit` int(11) DEFAULT '0',
  `floor` int(11) DEFAULT '0',
  `room` int(11) DEFAULT '0',
  `tel` varchar(64) DEFAULT NULL,
  `timeout` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `access_jpeg`
--

CREATE TABLE `access_jpeg` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `url` varchar(128) NOT NULL,
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `access_logger`
--

CREATE TABLE `access_logger` (
  `id` int(11) NOT NULL,
  `card` varchar(64) NOT NULL,
  `user` varchar(64) NOT NULL,
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `aclsavp`
--

CREATE TABLE `aclsavp` (
  `attr` varchar(255) NOT NULL,
  `value` varchar(4096) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `ad_logger`
--

CREATE TABLE `ad_logger` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `enable` int(11) DEFAULT '0',
  `url` varchar(512) NOT NULL,
  `done` int(11) DEFAULT '0',
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `ad_store`
--

CREATE TABLE `ad_store` (
  `id` int(11) NOT NULL,
  `store` int(11) DEFAULT '0',
  `name` varchar(128) NOT NULL,
  `url` varchar(128) NOT NULL,
  `expire` int(11) DEFAULT '5',
  `start` datetime DEFAULT NULL,
  `stop` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `ad_wakeup`
--

CREATE TABLE `ad_wakeup` (
  `id` int(11) NOT NULL,
  `name` varchar(128) NOT NULL,
  `timeout` int(11) DEFAULT '300',
  `store` int(11) DEFAULT '0',
  `url` varchar(512) NOT NULL,
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `alarm_content`
--

CREATE TABLE `alarm_content` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `zone` int(11) NOT NULL,
  `content` varchar(8192) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `alarm_defence`
--

CREATE TABLE `alarm_defence` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `defence` int(11) NOT NULL,
  `io` varchar(512) DEFAULT NULL,
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `alarm_logger`
--

CREATE TABLE `alarm_logger` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `zone` int(11) NOT NULL,
  `data` int(11) NOT NULL,
  `ts` datetime DEFAULT NULL,
  `confirm` int(11) DEFAULT '0',
  `confirmTs` datetime DEFAULT NULL,
  `confirmUser` varchar(64) DEFAULT NULL,
  `confirmText` varchar(512) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `alarm_sensor`
--

CREATE TABLE `alarm_sensor` (
  `id` int(11) NOT NULL,
  `sensor` int(11) DEFAULT '0',
  `name` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- 資料表的匯出資料 `alarm_sensor`
--

INSERT INTO `alarm_sensor` (`id`, `sensor`, `name`) VALUES
(1, 0, '烟感'),
(2, 1, '煤气'),
(3, 2, '红外'),
(4, 3, '门磁'),
(5, 4, '窗磁'),
(6, 5, '紧急按钮'),
(7, 6, '水浸'),
(8, 7, '紧急绳索'),
(9, 8, '床头按钮');

-- --------------------------------------------------------

--
-- 資料表結構 `alarm_zone`
--

CREATE TABLE `alarm_zone` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `zone` int(11) NOT NULL,
  `name` varchar(256) NOT NULL,
  `level` int(11) DEFAULT '0',
  `sensor` int(11) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- 資料表的匯出資料 `alarm_zone`
--

INSERT INTO `alarm_zone` (`id`, `user`, `zone`, `name`, `level`, `sensor`) VALUES
(81, '101011111', 0, 'zone01', 5, 2),
(82, '101011111', 1, 'zone02', 5, 2),
(83, '101011111', 2, 'zone03', 5, 2),
(84, '101011111', 3, 'zone04', 5, 2),
(85, '101011111', 4, 'zone05', 5, 2),
(86, '101011111', 5, 'zone06', 5, 2),
(87, '101011111', 6, 'zone07', 5, 2),
(88, '101011111', 7, 'zone08', 5, 2),
(89, '101011111', 8, 'zone09', 5, 2),
(90, '101011111', 9, 'zone10', 5, 2),
(91, '101011111', 10, 'zone11', 5, 2),
(92, '101011111', 11, 'zone12', 5, 2),
(93, '101011111', 12, 'zone13', 5, 2),
(94, '101011111', 13, 'zone14', 5, 2),
(95, '101011111', 14, 'zone15', 5, 2),
(96, '101011111', 15, 'zone16', 5, 2),
(97, '120012222', 0, 'zone01', 5, 2),
(98, '120012222', 1, 'zone02', 5, 2),
(99, '120012222', 2, 'zone03', 5, 2),
(100, '120012222', 3, 'zone04', 5, 2),
(101, '120012222', 4, 'zone05', 5, 2),
(102, '120012222', 5, 'zone06', 5, 2),
(103, '120012222', 6, 'zone07', 5, 2),
(104, '120012222', 7, 'zone08', 5, 2),
(105, '120012222', 8, 'zone09', 5, 2),
(106, '120012222', 9, 'zone10', 5, 2),
(107, '120012222', 10, 'zone11', 5, 2),
(108, '120012222', 11, 'zone12', 5, 2),
(109, '120012222', 12, 'zone13', 5, 2),
(110, '120012222', 13, 'zone14', 5, 2),
(111, '120012222', 14, 'zone15', 5, 2),
(112, '120012222', 15, 'zone16', 5, 2);

-- --------------------------------------------------------

--
-- 資料表結構 `configsavp`
--

CREATE TABLE `configsavp` (
  `attr` varchar(255) NOT NULL,
  `value` varchar(4096) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `devices`
--

CREATE TABLE `devices` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `idx` int(11) DEFAULT '0',
  `url` varchar(128) DEFAULT NULL,
  `proxy_url` varchar(128) DEFAULT NULL,
  `proxy_ok` int(11) DEFAULT NULL,
  `heartbeat` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `filtersavp`
--

CREATE TABLE `filtersavp` (
  `attr` varchar(255) NOT NULL,
  `value` varchar(4096) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `groups`
--

CREATE TABLE `groups` (
  `id` int(11) NOT NULL,
  `parent` int(11) DEFAULT '0',
  `name` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- 資料表的匯出資料 `groups`
--

INSERT INTO `groups` (`id`, `parent`, `name`) VALUES
(1, 0, 'XM'),
(2, 0, 'TEST002'),
(3, 0, 'TEST002'),
(4, 0, 'TEST002'),
(5, 0, 'TEST002'),
(6, 0, 'TEST002');

-- --------------------------------------------------------

--
-- 資料表結構 `patrol_device`
--

CREATE TABLE `patrol_device` (
  `id` int(11) NOT NULL,
  `rid` int(11) DEFAULT NULL,
  `user` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `patrol_sched`
--

CREATE TABLE `patrol_sched` (
  `id` int(11) NOT NULL,
  `rid` int(11) DEFAULT NULL,
  `name` varchar(256) NOT NULL,
  `start` varchar(64) NOT NULL,
  `end` varchar(64) NOT NULL,
  `cycle` int(11) DEFAULT '60'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `patrol_user`
--

CREATE TABLE `patrol_user` (
  `id` int(11) NOT NULL,
  `rid` int(11) DEFAULT NULL,
  `card` varchar(64) NOT NULL,
  `format` int(11) DEFAULT NULL,
  `duty` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `push_id`
--

CREATE TABLE `push_id` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `idx` int(11) DEFAULT '0',
  `sid` varchar(128) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `routesavp`
--

CREATE TABLE `routesavp` (
  `attr` varchar(255) NOT NULL,
  `value` varchar(4096) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `settings`
--

CREATE TABLE `settings` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `idx` int(11) DEFAULT '0',
  `mac` varchar(64) NOT NULL,
  `lan_ip` varchar(64) NOT NULL,
  `lan_mask` varchar(64) NOT NULL,
  `lan_gateway` varchar(64) NOT NULL,
  `lan_dns` varchar(64) NOT NULL,
  `sip_enable` int(11) DEFAULT '0',
  `sip_proxy` varchar(64) DEFAULT NULL,
  `sip_realm` varchar(64) DEFAULT NULL,
  `sip_user` varchar(64) DEFAULT NULL,
  `sip_passwd` varchar(64) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `siloavp`
--

CREATE TABLE `siloavp` (
  `attr` varchar(255) NOT NULL,
  `attr2` varchar(255) NOT NULL,
  `value` varchar(20315) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `sms_text`
--

CREATE TABLE `sms_text` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `tel` varchar(64) NOT NULL,
  `text` varchar(256) NOT NULL,
  `done` int(11) DEFAULT '0',
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `sms_user`
--

CREATE TABLE `sms_user` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `name` varchar(64) NOT NULL,
  `tel` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `staticregsavp`
--

CREATE TABLE `staticregsavp` (
  `attr` varchar(255) NOT NULL,
  `value` varchar(4096) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `talk_logger`
--

CREATE TABLE `talk_logger` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `to_user` varchar(64) NOT NULL,
  `mode` int(11) DEFAULT '0',
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `text_logger`
--

CREATE TABLE `text_logger` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `type` int(11) DEFAULT '0',
  `text` varchar(512) NOT NULL,
  `done` int(11) DEFAULT '0',
  `ts` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 資料表結構 `types`
--

CREATE TABLE `types` (
  `id` int(11) NOT NULL,
  `tid` int(11) NOT NULL,
  `name` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- 資料表的匯出資料 `types`
--

INSERT INTO `types` (`id`, `tid`, `name`) VALUES
(1, 1, '分机'),
(2, 2, '门口机'),
(3, 3, '小门口机'),
(4, 4, '围墙机'),
(5, 5, '管理机'),
(6, 6, '普通用户'),
(7, 7, '系统管理员');

-- --------------------------------------------------------

--
-- 資料表結構 `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `user` varchar(64) NOT NULL,
  `passwd` varchar(32) DEFAULT NULL,
  `name` varchar(256) DEFAULT NULL,
  `forwardAddress` varchar(128) DEFAULT NULL,
  `heartbeat` datetime DEFAULT NULL,
  `online` int(11) DEFAULT '0',
  `gid` int(11) DEFAULT '0',
  `tid` int(11) DEFAULT '1',
  `build` int(11) DEFAULT '0',
  `unit` int(11) DEFAULT '0',
  `floor` int(11) DEFAULT '0',
  `room` int(11) DEFAULT '0',
  `idx` int(11) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- 資料表的匯出資料 `users`
--

INSERT INTO `users` (`id`, `user`, `passwd`, `name`, `forwardAddress`, `heartbeat`, `online`, `gid`, `tid`, `build`, `unit`, `floor`, `room`, `idx`) VALUES
(1, 'admin', '123456', 'admin', NULL, NULL, 0, 1, 7, 0, 0, 0, 0, 0),
(8, '101011111', '123456', '1-1-11111', NULL, NULL, 0, 1, 1, 1, 1, 1, 1111, NULL),
(9, '120012222', '123456', '120-1-2222', NULL, NULL, 0, 1, 1, 120, 1, 22, 22, NULL);

--
-- 已匯出資料表的索引
--

--
-- 資料表索引 `access_auth`
--
ALTER TABLE `access_auth`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `idx_card_user` (`card`,`user`);

--
-- 資料表索引 `access_card`
--
ALTER TABLE `access_card`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `card` (`card`);

--
-- 資料表索引 `access_jpeg`
--
ALTER TABLE `access_jpeg`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `access_logger`
--
ALTER TABLE `access_logger`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `aclsavp`
--
ALTER TABLE `aclsavp`
  ADD PRIMARY KEY (`attr`);

--
-- 資料表索引 `ad_logger`
--
ALTER TABLE `ad_logger`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `ad_store`
--
ALTER TABLE `ad_store`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `ad_wakeup`
--
ALTER TABLE `ad_wakeup`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `alarm_content`
--
ALTER TABLE `alarm_content`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `idx_user_zone` (`user`,`zone`);

--
-- 資料表索引 `alarm_defence`
--
ALTER TABLE `alarm_defence`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `alarm_logger`
--
ALTER TABLE `alarm_logger`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `alarm_sensor`
--
ALTER TABLE `alarm_sensor`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `alarm_zone`
--
ALTER TABLE `alarm_zone`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `idx_user_zone` (`user`,`zone`);

--
-- 資料表索引 `configsavp`
--
ALTER TABLE `configsavp`
  ADD PRIMARY KEY (`attr`);

--
-- 資料表索引 `devices`
--
ALTER TABLE `devices`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `idx_user_idx` (`user`,`idx`);

--
-- 資料表索引 `filtersavp`
--
ALTER TABLE `filtersavp`
  ADD PRIMARY KEY (`attr`);

--
-- 資料表索引 `groups`
--
ALTER TABLE `groups`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `patrol_device`
--
ALTER TABLE `patrol_device`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `user_rid` (`user`,`rid`);

--
-- 資料表索引 `patrol_sched`
--
ALTER TABLE `patrol_sched`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `patrol_user`
--
ALTER TABLE `patrol_user`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `card_rid` (`card`,`rid`);

--
-- 資料表索引 `push_id`
--
ALTER TABLE `push_id`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `sid` (`sid`),
  ADD UNIQUE KEY `idx_user_idx` (`user`,`idx`);

--
-- 資料表索引 `routesavp`
--
ALTER TABLE `routesavp`
  ADD PRIMARY KEY (`attr`);

--
-- 資料表索引 `settings`
--
ALTER TABLE `settings`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `mac` (`mac`),
  ADD UNIQUE KEY `lan_ip` (`lan_ip`),
  ADD UNIQUE KEY `idx_user_idx` (`user`,`idx`);

--
-- 資料表索引 `siloavp`
--
ALTER TABLE `siloavp`
  ADD PRIMARY KEY (`attr`),
  ADD KEY `SECONDARY` (`attr2`);

--
-- 資料表索引 `sms_text`
--
ALTER TABLE `sms_text`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `sms_user`
--
ALTER TABLE `sms_user`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `staticregsavp`
--
ALTER TABLE `staticregsavp`
  ADD PRIMARY KEY (`attr`);

--
-- 資料表索引 `talk_logger`
--
ALTER TABLE `talk_logger`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `text_logger`
--
ALTER TABLE `text_logger`
  ADD PRIMARY KEY (`id`);

--
-- 資料表索引 `types`
--
ALTER TABLE `types`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `tid` (`tid`);

--
-- 資料表索引 `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `user` (`user`);

--
-- 在匯出的資料表使用 AUTO_INCREMENT
--

--
-- 使用資料表 AUTO_INCREMENT `access_auth`
--
ALTER TABLE `access_auth`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `access_card`
--
ALTER TABLE `access_card`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `access_jpeg`
--
ALTER TABLE `access_jpeg`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `access_logger`
--
ALTER TABLE `access_logger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `ad_logger`
--
ALTER TABLE `ad_logger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `ad_store`
--
ALTER TABLE `ad_store`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `ad_wakeup`
--
ALTER TABLE `ad_wakeup`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `alarm_content`
--
ALTER TABLE `alarm_content`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `alarm_defence`
--
ALTER TABLE `alarm_defence`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `alarm_logger`
--
ALTER TABLE `alarm_logger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `alarm_sensor`
--
ALTER TABLE `alarm_sensor`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;
--
-- 使用資料表 AUTO_INCREMENT `alarm_zone`
--
ALTER TABLE `alarm_zone`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=113;
--
-- 使用資料表 AUTO_INCREMENT `devices`
--
ALTER TABLE `devices`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `groups`
--
ALTER TABLE `groups`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
--
-- 使用資料表 AUTO_INCREMENT `patrol_device`
--
ALTER TABLE `patrol_device`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `patrol_sched`
--
ALTER TABLE `patrol_sched`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `patrol_user`
--
ALTER TABLE `patrol_user`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `push_id`
--
ALTER TABLE `push_id`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `settings`
--
ALTER TABLE `settings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `sms_text`
--
ALTER TABLE `sms_text`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `sms_user`
--
ALTER TABLE `sms_user`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `talk_logger`
--
ALTER TABLE `talk_logger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `text_logger`
--
ALTER TABLE `text_logger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- 使用資料表 AUTO_INCREMENT `types`
--
ALTER TABLE `types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
--
-- 使用資料表 AUTO_INCREMENT `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
