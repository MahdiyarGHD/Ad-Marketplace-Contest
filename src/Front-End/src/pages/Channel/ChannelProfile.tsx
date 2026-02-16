import { memo, useEffect, useState } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../../components/PageHeader";
import Avatar from "../../components/Avatar";
import { useNavigate, useParams } from "react-router-dom";
import { requestAPI } from "../../utils/api";
import useChannelStore, {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type Channel,
	type PriceType,
} from "../../stores/useChannelStore";
import { useShallow } from "zustand/shallow";
import { backButton, openTelegramLink } from "@tma.js/sdk-react";
import { buildClassName, invokeHapticFeedbackImpact } from "../../utils/common";
import "../Statistics.scss";
import { Shimmer } from "../../components/Shimmer";
import Transition from "../../components/Transition";
import {
	ChevronRight,
	ChevronRightIcon,
	EllipsisVerticalIcon,
	PencilIcon,
	TagIcon,
} from "lucide-react";
import useCategoryStore from "../../stores/useCategoryStore";
import useAppStore from "../../stores/useAppStore";
import useUIStore from "../../stores/useUIStore";
import Modal from "../../components/Modal";
import Application from "../../components/Application";
import useApplicationStore from "../../stores/useApplicationStore";

function ChannelProfile() {
	const [showApplication, setShowApplication] = useState<boolean>(false);
	const [applicationType, setApplicationType] = useState<"apply" | "invite">();

	const { id } = useParams();

	const {
		chat_id,
		owner_id,
		title,
		username,
		category_id,
		subscriber_count,
		average_views,
		premium_count,
		language_distribution_json,
		pricings,
	} = useChannelStore(useShallow((state) => state.activeChannel)) || {};
	const { setActiveChannel, setDraftChannel } = useChannelStore(
		useShallow((state) => ({
			setActiveChannel: state.setActiveChannel,
			setDraftChannel: state.setDraftChannel,
		})),
	);

	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const clearApplication = useApplicationStore(
		useShallow((state) => state.clearApplication),
	);

	const userId = useAppStore(useShallow((state) => state.userId));

	const isOwn = owner_id === userId;

	const { getCategory } = useCategoryStore();

	const category = getCategory(category_id ?? "");

	const invite = location.pathname.endsWith("/invite");

	const navigate = useNavigate();

	const getChannelInfo = async () => {
		const response = await requestAPI(`/api/channels/${id}`, {}, "GET");

		setActiveChannel(response.value);
		console.log(response);
	};

	const onBackButton = () => {
		window.history.back();
	};

	const onEdit = () => {
		setDraftChannel({
			id,
			chat_id,
			title,
			category_id,
			category,
			pricings,
		} as Channel);

		navigate(`/set-channel-data`);
	};

	const onApply = () => {
		if (isOwn) return;

		setApplication({
			channel_id: id,
			channel: {
				chat_id,
				owner_id,
				title,
				username,
				category_id,
				subscriber_count,
				average_views,
				pricings,
			} as Channel,
			proposed_ad_format: pricings?.[0]?.ad_format,
			proposed_price_type: pricings?.[0]?.price_type,
		});
		setApplicationType("apply");
		setShowApplication(true);
	};

	const onInvite = () => {
		if (isOwn) return;

		setApplication({
			channel_id: id,
			channel: {
				chat_id,
				owner_id,
				title,
				username,
				category_id,
				subscriber_count,
				average_views,
				pricings,
			} as Channel,
			proposed_ad_format: pricings?.[0]?.ad_format,
			proposed_price_type: pricings?.[0]?.price_type,
		});
		setApplicationType("invite");
		setShowApplication(true);
	};

	const onApplicationClose = () => {
		setShowApplication(false);

		clearApplication();
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (!id) return;

		getChannelInfo();
	}, [id]);

	useEffect(() => {
		if (owner_id && !isOwn) {
			useUIStore.setState({
				mainButton: { text: "Apply", onClick: onApply },
				textButton: { text: "Invite to campaign", onClick: onInvite },
			});
		} else {
			useUIStore.setState({ mainButton: undefined, textButton: undefined });
		}
	}, [isOwn, owner_id]);

	useEffect(() => {
		if (invite) {
			setApplicationType("invite");
			setShowApplication(true);
		}
	}, [invite]);

	return (
		<div className="scrollable">
			<div className="ChannelProfile Profile">
				<PageHeader>
					<PageHeaderTitle> </PageHeaderTitle>
					<PageHeaderButtons>
						{isOwn && (
							<div className="Edit" onClick={onEdit}>
								<PencilIcon size={22} />
							</div>
						)}
						<div className="More">
							<EllipsisVerticalIcon />
						</div>
					</PageHeaderButtons>
				</PageHeader>

				<div className="Chat">
					<Avatar id={chat_id!} title={title ?? "C"} photo="" size={80} />
					<div className="info">
						<div className={buildClassName("title", !title && "Shimmer")}>
							{title}
						</div>
						<div
							className="subtitle pointer"
							onClick={() =>
								username && openTelegramLink(`https://t.me/${username}`)
							}
						>
							{username ? `@${username}` : "private channel"}{" "}
							{username && <ChevronRightIcon size={18} />}
						</div>
					</div>
				</div>

				<div className="Items">
					<div
						className="Item"
						onClick={() => navigate(`/category/${category_id}`)}
					>
						<div className="icon">{category?.icon ?? <TagIcon />}</div>
						<div className="body">
							<Shimmer className="title">{category?.name}</Shimmer>
						</div>
					</div>
				</div>

				<div className="Statistics">
					<div className="Item">
						<Shimmer className="title" state={subscriber_count !== undefined}>
							<span>{subscriber_count}</span>
						</Shimmer>
						<div className="subtitle">Subscribers</div>
					</div>
					<div className="Item">
						<Shimmer className="title" state={average_views !== undefined}>
							<span>{average_views}</span>
						</Shimmer>
						<div className="subtitle">Average Views</div>
					</div>
					{!!language_distribution_json?.length && (
						<div className="Item">
							<Shimmer
								className="title"
								state={language_distribution_json !== undefined}
							>
								<span>{language_distribution_json?.[0]?.language}</span>
							</Shimmer>
							<div className="subtitle">Top Language</div>
						</div>
					)}
					<div className="Item">
						<Shimmer className="title" state={premium_count !== undefined}>
							<span>{premium_count}</span>
						</Shimmer>
						<div className="subtitle">Premium Subscribers</div>
					</div>
				</div>

				{isOwn && (
					<div className="Section">
						<div className="title">Manage</div>
						<div className="Items">
							<div
								className="Item"
								onClick={() => navigate(`/channel/${id}/applications`)}
							>
								<div className="body">
									<div className="title">Applications</div>
								</div>
								<div className="meta">
									<ChevronRight />
								</div>
							</div>
							<div
								className="Item"
								onClick={() => navigate(`/channel/${id}/invitations`)}
							>
								<div className="body">
									<div className="title">Invitations</div>
								</div>
								<div className="meta">
									<ChevronRight />
								</div>
							</div>
						</div>
					</div>
				)}

				<div className="Section">
					<div className="title">Pricing</div>
					<div
						className={buildClassName(
							"StatItems",
							!pricings?.length && "loading",
						)}
					>
						<Transition
							state
							key={pricings?.length}
							eachElement
							eachElementDelay={100}
						>
							{(pricings ?? Array.from({ length: 1 })).map((price, index) => (
								// biome-ignore lint/suspicious/noArrayIndexKey: <explanation>
								<div className="Item" key={index}>
									<div className="flex-1">
										{/* <DollarSign /> */}
										<Shimmer className="title" state={!!price?.price_ton}>
											{price?.price_ton.toFixed(2)} TON
										</Shimmer>
										<div className="subtitle">Price</div>
									</div>
									<div className="flex-1">
										<Shimmer className="title" state={!!price?.price_type}>
											{PriceTypes[price?.price_type as PriceType]}
										</Shimmer>
										<div className="subtitle">Price type</div>
									</div>
									<div>
										<Shimmer className="title" state={!!price?.ad_format}>
											{AdFormats[price?.ad_format as AdFormat]}
										</Shimmer>
										<div className="subtitle">Ad format</div>
									</div>
								</div>
							))}
						</Transition>
					</div>
				</div>

				<Modal
					open={showApplication}
					onClose={onApplicationClose}
					title={
						applicationType === "apply" ? "Application" : "Invite Proposal"
					}
				>
					<Application
						type={applicationType === "invite" ? "invite" : "channel"}
						onClose={onApplicationClose}
					/>
				</Modal>
			</div>
		</div>
	);
}

export default memo(ChannelProfile);
