-- 创建 schema（如果不存在）
CREATE SCHEMA IF NOT EXISTS `LoveEnglish`;

-- 使用 schema
USE `LoveEnglish`;

-- word.words definition

-- 使用test数据库
USE `LoveEnglish`;

-- 创建words表（如果不存在）按照读取顺序调整字段
CREATE TABLE IF NOT EXISTS `words` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT '索引',
  `WordName` varchar(100) NOT NULL COMMENT '单词名称',
  `BritishPhonetic` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '英式英标',
  `AmericanPhonetic` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '美式音标',
  `Translation` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '翻译',
  `Exchange` TEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Examples` TEXT DEFAULT NULL COMMENT '例句',
  `IsNewWord` tinyint(1) DEFAULT NULL COMMENT '是否为生词',
  `NextReviewDate` timestamp NULL DEFAULT NULL COMMENT '下次复习日期',
  `IsFavorite` tinyint(1) DEFAULT NULL COMMENT '是否收藏',
  `ErrorCount` int DEFAULT NULL COMMENT '错误次数',
  `DictTag` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '属于的词典',
  `ExampleTrans` varchar(100) DEFAULT NULL COMMENT '例句翻译',
  `ExampleSource` varchar(100) DEFAULT NULL COMMENT '例句来源',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `words_unique` (`WordName`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 批量插入初始数据（按照读取顺序调整字段）

INSERT INTO `words` (
  `Id`, `WordName`, `BritishPhonetic`, `AmericanPhonetic`, `Translation`, 
  `Exchange`, `Examples`, `IsNewWord`, `NextReviewDate`, `IsFavorite`, 
  `ErrorCount`, `DictTag`, `ExampleTrans`, `ExampleSource`
) VALUES
(1, 'apple', '/ˈæpl/', '/ˈæpəl/', 'n. 苹果', '复数:apples', 
'I eat an apple every day.\nAn apple a day keeps the doctor away.\nShe gave me a red apple.', 
1, '2025-05-01 09:00:00', 1, 0, 'CET4 四级词汇', 
'我每天吃一个苹果。\n一天一苹果，医生远离我。\n她给了我一个红苹果。', 
'日常生活\n谚语\n个人对话'),

(2, 'run', '/rʌn/', '/rʌn/', 'v. 跑步', '过去式:ran\n过去分词:run\n现在分词:running', 
'He runs every morning.\nI ran a marathon last year.\nShe is running in the park now.', 
1, '2025-05-02 09:00:00', 0, 0, 'CET4 四级词汇', 
'他每天早上跑步。\n我去年跑了一场马拉松。\n她现在正在公园里跑步。', 
'日常习惯\n体育赛事\n当前活动'),

(3, 'beautiful', '/ˈbjuːtɪf(ə)l/', '/ˈbjuːtɪf(ə)l/', 'adj. 美丽的', '比较级:more beautiful\n最高级:most beautiful', 
'The sunset is beautiful.\nShe wore a beautiful dress.\nThis is the most beautiful place I have ever seen.', 
1, '2025-05-03 09:00:00', 1, 0, 'CET6 六级词汇', 
'日落很美。\n她穿了一件漂亮的裙子。\n这是我见过最美的地方。', 
'自然景观\n服装描述\n旅行见闻'),

(4, 'quickly', '/ˈkwɪkli/', '/ˈkwɪkli/', 'adv. 快速地', NULL, 
'She finished quickly.\nTime passes quickly when you are happy.\nPlease come here quickly.', 
0, NULL, 0, 0, 'TOEFL 托福词汇', 
'她很快就完成了。\n当你快乐时，时间过得很快。\n请快点过来。', 
'工作场景\n生活感悟\n紧急情况'),

(5, 'between', '/bɪˈtwiːn/', '/bɪˈtwiːn/', 'prep. 在...之间', NULL, 
'Sit between us.\nThe meeting is between 2pm and 4pm.\nThere is a strong bond between them.', 
0, NULL, 0, 0, 'IELTS 雅思词汇', 
'坐在我们中间。\n会议在下午2点到4点之间。\n他们之间有很强的纽带。', 
'空间关系\n时间安排\n人际关系'),

(6, 'ephemeral', '/ɪˈfemərəl/', '/ɪˈfemərəl/', 'adj. 短暂的', NULL, 
'Ephemeral beauty of flowers.\nFame can be ephemeral.\nThe mayfly has an ephemeral life.', 
1, '2025-05-10 09:00:00', 1, 0, 'GRE 研究生词汇', 
'花朵的短暂美丽。\n名声可能是短暂的。\n蜉蝣的生命很短暂。', 
'自然观察\n社会现象\n生物学'),

(7, 'serendipity', '/ˌserənˈdɪpəti/', '/ˌserənˈdɪpəti/', 'n. 意外发现', NULL, 
'A happy serendipity.\nThe discovery was pure serendipity.\nThey met by serendipity.', 
1, '2025-05-11 09:00:00', 1, 0, 'GRE 研究生词汇', 
'一个愉快的意外发现。\n这个发现纯属偶然。\n他们偶然相遇。', 
'个人经历\n科学发现\n人际关系'),

(8, 'ubiquitous', '/juːˈbɪkwɪtəs/', '/juːˈbɪkwɪtəs/', 'adj. 无处不在的', NULL, 
'Smartphones are ubiquitous.\nThe ubiquitous presence of advertising.\nCoffee shops are ubiquitous in this city.', 
1, '2025-05-12 09:00:00', 0, 0, 'TOEFL 托福词汇', 
'智能手机无处不在。\n广告的无处不在。\n咖啡店在这座城市随处可见。', 
'科技产品\n商业现象\n城市生活'),

(9, 'eloquent', '/ˈeləkwənt/', '/ˈeləkwənt/', 'adj. 雄辩的', NULL, 
'An eloquent speaker.\nHer eloquent defense moved the jury.\nThe poem is eloquent in its simplicity.', 
0, NULL, 0, 0, 'IELTS 雅思词汇', 
'一位雄辩的演讲者。\n她雄辩的辩护打动了陪审团。\n这首诗以其简洁而雄辩。', 
'公众演讲\n法庭场景\n文学评论'),

(10, 'nostalgia', '/nɒˈstældʒə/', '/nəˈstældʒə/', 'n. 怀旧', NULL, 
'Feeling of nostalgia.\nThe movie evoked nostalgia for the 80s.\nOld photos often bring nostalgia.', 
1, '2025-05-14 09:00:00', 1, 0, 'GRE 研究生词汇', 
'怀旧的感觉。\n这部电影唤起了对80年代的怀念。\n旧照片常常带来怀旧之情。', 
'情感体验\n影视作品\n个人记忆');



-- 在loveenglish数据库中创建questions表
CREATE TABLE IF NOT EXISTS questions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Content varchar(100) NOT NULL COMMENT '问题内容',
    WordId INT NOT NULL COMMENT '关联words表的外键',
    Type VARCHAR(50) COMMENT '问题类型',
    Answer varchar(100) COMMENT '问题答案',
    CONSTRAINT fk_questions_wordid 
    FOREIGN KEY (WordId) REFERENCES loveenglish.words(Id)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='问题表';
-- 为questions表插入测试数据
INSERT INTO questions (Content, WordId, Type, Answer) VALUES
-- apple (WordId: 1)
('这个水果的英文名称是什么？', 1, '单词识别', 'apple'),
('请拼写这个单词：/ˈæpl/', 1, '拼写测试', 'a-p-p-l-e'),
('"apple"的美式发音是什么？', 1, '发音测试', '/ˈæpəl/'),

-- run (WordId: 2)
('"run"的过去式是什么？', 2, '词形变化', 'ran'),
('"run"在句子"He ___ five miles every day."中应该用什么形式？', 2, '语法填空', 'runs'),
('"run"的英式发音是什么？', 2, '发音测试', '/rʌn/'),

-- beautiful (WordId: 3)
('"beautiful"的比较级是什么？', 3, '词形变化', 'more beautiful'),
('"beautiful"的反义词是什么？', 3, '词汇关系', 'ugly'),
('"beautiful"的英式发音是什么？', 3, '发音测试', '/ˈbjuːtɪf(ə)l/'),

-- quickly (WordId: 4)
('"quickly"的词根是什么？', 4, '词源分析', 'quick'),
('"quickly"在句子"She finished her homework ___."中应该用什么形式？', 4, '语法填空', 'quickly'),
('"quickly"的美式发音是什么？', 4, '发音测试', '/ˈkwɪkli/'),

-- between (WordId: 5)
('"between"通常用于描述几个事物之间的关系？', 5, '用法理解', '两个'),
('"between"在句子"The book is ___ the lamp and the photo frame."中应该用什么形式？', 5, '语法填空', 'between'),
('"between"的英式发音是什么？', 5, '发音测试', '/bɪˈtwiːn/');


CREATE TABLE IF NOT EXISTS `articles` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT '文章ID',
  `Title` varchar(255) NOT NULL COMMENT '文章标题',
  `Content` TEXT NOT NULL COMMENT '文章内容',
  `SourceUrl` varchar(255) DEFAULT NULL COMMENT '原始文章链接',
  `CreatedTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='英语学习文章表';

-- 插入示例数据
INSERT INTO `articles` (`Title`, `Content`, `SourceUrl`, `CreatedTime`) VALUES
('Effective English Learning', 'Here are five proven methods to learn English...', 'https://example.com/english-learning', '2023-05-15 10:00:00'),
('Common Grammar Mistakes', 'Learn how to avoid the 10 most common English grammar mistakes...', 'https://example.com/grammar-mistakes', '2023-06-20 14:30:00'),
('Business English Phrases', 'Essential phrases for professional communication...', NULL, '2023-07-10 09:15:00'),
('Youth','Youth is not a time of life; it is a state of mind; it is not a matter of rosy cheeks, red lips and supple knees; it is a matter of the will, a quality of the imagination, a vigor of the emotions; it is the freshness of the deep springs of life.Youth means a temperamental predominance of courage over timidity, of the appetite for adventure over the love of ease. This often exists in a man of 60 more than a boy of 20. Nobody grows old merely by a number of years. We grow old by deserting our ideals.',
NULL,'2023-07-10 09:15:00');



CREATE TABLE IF NOT EXISTS loveenglish.users (
    Id INT AUTO_INCREMENT NOT NULL,
    CheckInDays INT NULL COMMENT '连续打卡时间',
    LastLoginTime TIMESTAMP NULL COMMENT '上次登录日期',
    CONSTRAINT users_pk PRIMARY KEY (Id)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_0900_ai_ci;


INSERT INTO loveenglish.users (CheckInDays, LastLoginTime) 
VALUES (7, '2025-05-07 09:00:00');




CREATE TABLE IF NOT EXISTS loveenglish.dictionaries (
    Id INT AUTO_INCREMENT NOT NULL COMMENT '词典ID',
    DictName VARCHAR(100) NOT NULL COMMENT '词库名称',
    CONSTRAINT dictionaries_pk PRIMARY KEY (Id)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_0900_ai_ci;

-- 插入词典数据 (基于图片中的DictTag值)
INSERT INTO loveenglish.dictionaries (DictName) 
VALUES 
    ('CET4 四级词汇'),
    ('CET6 六级词汇'), 
    ('TOEFL 托福词汇'),
    ('IELTS 雅思词汇'),
    ('GRE 研究生词汇');